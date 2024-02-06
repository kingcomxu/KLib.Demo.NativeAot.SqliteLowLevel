using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestSqlite;

internal class Program
{
    static unsafe void Main(string[] args)
    { 
        var ptrV = Sqlite3.sqlite3_libversion();
        var str = Marshal.PtrToStringUTF8(ptrV);
        Console.WriteLine($"Sqlite Version:{str}");


        void* pDb;

        int result = 0;
        var ptrFileName = Marshal.StringToCoTaskMemUTF8("test.db");
        result = Sqlite3.sqlite3_open((void*)ptrFileName, &pDb);
        Marshal.FreeCoTaskMem(ptrFileName);

        if (result == Sqlite3.SQLITE_OK)
        {
            Console.WriteLine("opened database successfully!");
        }
        else
        {
            var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
            var errMsg = Marshal.PtrToStringUTF8(ptrErrMsg);
            Console.WriteLine($"open result:0x{result:X},  Can't open database: {errMsg}");
        }


        string sql = "CREATE TABLE COMPANY( ID INT PRIMARY KEY NOT NULL, NAME TEXT    NOT NULL, AGE INT NOT NULL, ADDRESS CHAR(50), SALARY REAL );";

        Console.WriteLine($"CREATE TABLE COMPANY result:{Sqlite3.ExecuteSql(pDb, sql)}");


        //insert data
        sql = "INSERT INTO COMPANY (ID,NAME,AGE,ADDRESS,SALARY) " +
         "VALUES (1, 'Paul', 32, 'California', 20000.00 ); " +
         "INSERT INTO COMPANY (ID,NAME,AGE,ADDRESS,SALARY) " +
         "VALUES (2, 'Allen', 25, 'Texas', 15000.00 ); " +
         "INSERT INTO COMPANY (ID,NAME,AGE,ADDRESS,SALARY)" +
         "VALUES (3, 'Teddy', 23, 'Norway', 20000.00 );" +
         "INSERT INTO COMPANY (ID,NAME,AGE,ADDRESS,SALARY)" +
         "VALUES (4, 'Mark', 25, 'Rich-Mond ', 65000.00 );";

        Console.WriteLine($"INSERT INTO COMPANY result:{Sqlite3.ExecuteSql(pDb, sql)}");


        var ptrData = Marshal.StringToCoTaskMemUTF8("select * from Company");

        //select rows by callback
        sql = "SELECT * from COMPANY";
        void* pErrMsg;
        var ptrSql = Marshal.StringToCoTaskMemUTF8(sql);
        result = Sqlite3.sqlite3_exec(pDb, (void*)ptrSql, (nint)(delegate*<nint, int, byte**, byte**, int>)&callback, (void*)ptrData, &pErrMsg);
        Marshal.FreeCoTaskMem(ptrSql);

        Marshal.FreeCoTaskMem(ptrData);
        if (result != Sqlite3.SQLITE_OK)
        {
            Console.WriteLine($"Sql Exec Error:{Sqlite3.GetErrMsgFromPtr(pErrMsg)}");
        }
        else
        {
            Console.WriteLine("Select Rows successfully");
        }


        //get table 
        sql = "SELECT * from COMPANY";
        void** pResult;
        int nRow, nCol;
        ptrSql = Marshal.StringToCoTaskMemUTF8(sql);
        result = Sqlite3.sqlite3_get_table(pDb, (void*)ptrSql, &pResult, &nRow, &nCol, &pErrMsg);
        Marshal.FreeCoTaskMem(ptrSql);
        if (result != Sqlite3.SQLITE_OK)
        {
            Console.WriteLine($"Sql Exec Error:{Sqlite3.GetErrMsgFromPtr(pErrMsg)}");
        }
        else
        {
            //get data from pResult
            Console.WriteLine($"rows of data:{nRow}");
            Console.WriteLine($"cols of data:{nCol}");

            //output the column names
            for (int i = 0; i < nCol; i++)
            {
                var colName = Marshal.PtrToStringUTF8((nint)pResult[i]);
                Console.Write($"{colName}\t");
            }
            Console.WriteLine();

            //output the row data
            for (int i = 1; i < nCol; i++)
            {
                for (int colIndex = 0; colIndex < nCol; colIndex++)
                {
                    var colValue = Marshal.PtrToStringUTF8((nint)pResult[i * nCol + colIndex]);
                    Console.Write($"{colValue}\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            Sqlite3.sqlite3_free_table(pResult);
            Console.WriteLine("get table successfully");
        }

        //////test blob insert and read
        sql = "CREATE TABLE EmpPhoto3( ID INT PRIMARY KEY NOT NULL, NAME TEXT, PHOTO BLOB, F FLOAT);";
        Console.WriteLine($"CREATE TABLE EmpPhoto result:{Sqlite3.ExecuteSql(pDb, sql)}");

        //blob insert
        sql = "INSERT INTO EmpPhoto3(ID,NAME, PHOTO, F)values(?, ?, ?,?);";
        ptrSql = Marshal.StringToCoTaskMemUTF8(sql);
        void* stmt;
        result = Sqlite3.sqlite3_prepare(pDb, (void*)ptrSql, -1/*ptrsql length in byte*/, &stmt, (void**)0);
        Console.WriteLine($"sqlite3_prepare result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK}");

        List<string> fileNames = [Path.Combine(AppContext.BaseDirectory, "a.png"), Path.Combine(AppContext.BaseDirectory, "b.ico")];

        for (int i = 0; i < fileNames.Count; i++)
        {

            //bind Id
            result = Sqlite3.sqlite3_bind_int(stmt, 1, i);
            Console.WriteLine($"sqlite3_bind_int result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK}");
            if (result != Sqlite3.SQLITE_OK)
            {
                var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
                Console.WriteLine($"sqlite3_step Error:{Marshal.PtrToStringUTF8(ptrErrMsg)}");
            }

            //bind Name
            string name = $"name-{i}";
            var ptrName = Marshal.StringToCoTaskMemUTF8(name);
            result = Sqlite3.sqlite3_bind_text(stmt, 2, (void*)ptrName, -1, (void*)0);
            Marshal.FreeCoTaskMem(ptrName);
            Console.WriteLine($"sqlite3_bind_text result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK}");
            if (result != Sqlite3.SQLITE_OK)
            {
                var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
                Console.WriteLine($"sqlite3_step Error:{Marshal.PtrToStringUTF8(ptrErrMsg)}");
            }
            //bind blob

            var fileBytes = File.ReadAllBytes(fileNames[i]);
            var gchFileBytes = GCHandle.Alloc(fileBytes, GCHandleType.Pinned);

            result = Sqlite3.sqlite3_bind_blob(stmt, 3, (void*)gchFileBytes.AddrOfPinnedObject(), fileBytes.Length, (void*)0);
            Console.WriteLine($"sqlite3_bind_blob result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK}");
            gchFileBytes.Free();
            if (result != Sqlite3.SQLITE_OK)
            {
                var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
                Console.WriteLine($"sqlite3_step Error:{Marshal.PtrToStringUTF8(ptrErrMsg)}");
            }


            //bind f Float 

            var doubleValue = double.Parse($"{i}.{i}") + 55.1234;
            Console.WriteLine($"doubleValue:{doubleValue}");

            result = Sqlite3.sqlite3_bind_double(stmt, 4, doubleValue);
            Console.WriteLine($"sqlite3_bind_double result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK}");
            if (result != Sqlite3.SQLITE_OK)
            {
                var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
                Console.WriteLine($"sqlite3_step Error:{Marshal.PtrToStringUTF8(ptrErrMsg)}");
            }

            //step 

            result = Sqlite3.sqlite3_step(stmt);
            Console.WriteLine($"sqlite3_step result:{result}, result == SQLITE_DONE: {result == Sqlite3.SQLITE_DONE}");
            if (result == Sqlite3.SQLITE_ERROR)
            {
                var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
                Console.WriteLine($"sqlite3_step Error:{Marshal.PtrToStringUTF8(ptrErrMsg)}");
            }

            Console.WriteLine($"before reset,  valueof stmt:0x{(nint)stmt:X}");
            Console.WriteLine($"before reset,  valueof *stmt:0x{*(nint*)stmt:X}");
            result = Sqlite3.sqlite3_reset(stmt);
            Console.WriteLine($"sqlite3_reset result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK},valueof stmt:0x{(nint)stmt:X}");
            Console.WriteLine($"sqlite3_reset result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK},valueof *stmt:0x{*(nint*)stmt:X}");

        }

        result = Sqlite3.sqlite3_finalize(stmt);
        Console.WriteLine($"sqlite3_finalize result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK}");


        //begin read blob
        Console.WriteLine("begin read blob!");


        sql = "select * from empphoto3";
        ptrSql = Marshal.StringToCoTaskMemUTF8(sql);
        result = Sqlite3.sqlite3_prepare(pDb, (void*)ptrSql, -1/*ptrsql length in byte*/, &stmt, (void**)0);
        Marshal.FreeCoTaskMem(ptrSql);
        Console.WriteLine($"sqlite3_prepare result:{result}, result == sqlite_OK: {result == Sqlite3.SQLITE_OK}");
        if (result != Sqlite3.SQLITE_OK)
        {
            var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
            Console.WriteLine($"sqlite3_prepare Error:{Marshal.PtrToStringUTF8(ptrErrMsg)}");
        }
        result = Sqlite3.sqlite3_step(stmt);
        Console.WriteLine($"sqlite3_step result:{result}, result == SQLITE_DONE: {result == Sqlite3.SQLITE_DONE}");
        if (result == Sqlite3.SQLITE_ERROR)
        {
            var ptrErrMsg = Sqlite3.sqlite3_errmsg(pDb);
            Console.WriteLine($"sqlite3_step Error:{Marshal.PtrToStringUTF8(ptrErrMsg)}");
        }

        while (result == Sqlite3.SQLITE_ROW)
        { 
            //col 0 
            int id = Sqlite3.sqlite3_column_int(stmt, 0);
            Console.WriteLine(id);


            var ptrId = Sqlite3.sqlite3_column_blob(stmt, 0);
            var blobId = Marshal.PtrToStringUTF8((nint)ptrId);

            Console.WriteLine($"id from blod:{blobId}");

            //col 1
            var ptrStr = Sqlite3.sqlite3_column_text(stmt, 1);
            var name = Marshal.PtrToStringUTF8((nint)ptrStr);
            Console.WriteLine(name);

            var ptrStrBlob = Sqlite3.sqlite3_column_blob(stmt, 1);
            var nameBlob = Marshal.PtrToStringUTF8((nint)ptrStrBlob);
            Console.WriteLine($"name from blob:{nameBlob}");




            //col 2 blob 
            var ptrBlob = Sqlite3.sqlite3_column_blob(stmt, 2);
            var len = Sqlite3.sqlite3_column_bytes(stmt, 2);
            var spanBytes = new ReadOnlySpan<byte>(ptrBlob, len);
            var outputFilename = Path.Combine(AppContext.BaseDirectory, $"read-blob-result-{name}.png");
            using SafeFileHandle handle = File.OpenHandle(outputFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            RandomAccess.Write(handle, spanBytes, 0);
            Console.WriteLine($"blob.len:{len}, outputas:{outputFilename}");


            //col 3 float
            var ptrCol4Name = Sqlite3.sqlite3_column_name(stmt, 3);
            var col4Name = Marshal.PtrToStringUTF8((nint)ptrCol4Name);

            var fValue = Sqlite3.sqlite3_column_double(stmt, 3);
            Console.WriteLine($"sqlite3_column_double {col4Name}:{fValue}");

            //col 3 float by blob
            var ptrFBlob = Sqlite3.sqlite3_column_blob(stmt, 3);
            var blobF = Marshal.PtrToStringUTF8((nint)ptrFBlob);

            Console.WriteLine($"F from blod:{blobF}");



            //get next row data
            result = Sqlite3.sqlite3_step(stmt);

        }

        Sqlite3.sqlite3_finalize(stmt);


        //endof read blob

        Sqlite3.sqlite3_close(pDb);


        Console.ReadKey();
    }

    static unsafe int callback(nint ptrData, int colCount, byte** colValues, byte** colNames)
    {
        if (ptrData != 0)
        {
            string? data = Marshal.PtrToStringUTF8((nint)ptrData);
            Console.WriteLine($"custom data:{data}");
        }

        for (int i = 0; i < colCount; i++)
        {
            void* pColName = colNames[i];
            string? colName = Marshal.PtrToStringUTF8((nint)pColName);

            void* pColValue = colValues[i];
            string? colValue = Marshal.PtrToStringUTF8((nint)pColValue);

            Console.WriteLine($"{colName}:{colValue}");
        }

        Console.WriteLine("========================================================");


        return 0;
    }
}
