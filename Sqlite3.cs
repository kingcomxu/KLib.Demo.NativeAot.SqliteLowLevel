using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestSqlite;
public unsafe class Sqlite3
{
    public const int SQLITE_OK = 0;         /* Successful result */
    /* beginning-of-error-codes */
    public const int SQLITE_ERROR = 1;       /* Generic error */
    public const int SQLITE_INTERNAL = 2;       /* Internal logic error in SQLite */
    public const int SQLITE_PERM = 3;       /* Access permission denied */
    public const int SQLITE_ABORT = 4;       /* Callback routine requested an abort */
    public const int SQLITE_BUSY = 5;       /* The database file is locked */
    public const int SQLITE_LOCKED = 6;       /* A table in the database is locked */
    public const int SQLITE_NOMEM = 7;       /* A malloc() failed */
    public const int SQLITE_READONLY = 8;       /* Attempt to write a readonly database */
    public const int SQLITE_INTERRUPT = 9;       /* Operation terminated by sqlite3_interrupt()*/
    public const int SQLITE_IOERR = 10;       /* Some kind of disk I/O error occurred */
    public const int SQLITE_CORRUPT = 11;       /* The database disk image is malformed */
    public const int SQLITE_NOTFOUND = 12;       /* Unknown opcode in sqlite3_file_control() */
    public const int SQLITE_FULL = 13;       /* Insertion failed because database is full */
    public const int SQLITE_CANTOPEN = 14;       /* Unable to open the database file */
    public const int SQLITE_PROTOCOL = 15;       /* Database lock protocol error */
    public const int SQLITE_EMPTY = 16;       /* Internal use only */
    public const int SQLITE_SCHEMA = 17;       /* The database schema changed */
    public const int SQLITE_TOOBIG = 18;       /* String or BLOB exceeds size limit */
    public const int SQLITE_CONSTRAINT = 19;       /* Abort due to constraint violation */
    public const int SQLITE_MISMATCH = 20;       /* Data type mismatch */
    public const int SQLITE_MISUSE = 21;       /* Library used incorrectly */
    public const int SQLITE_NOLFS = 22;       /* Uses OS features not supported on host */
    public const int SQLITE_AUTH = 23;       /* Authorization denied */
    public const int SQLITE_FORMAT = 24;       /* Not used */
    public const int SQLITE_RANGE = 25;       /* 2nd parameter to sqlite3_bind out of range */
    public const int SQLITE_NOTADB = 26;       /* File opened that is not a database file */
    public const int SQLITE_NOTICE = 27;       /* Notifications from sqlite3_log() */
    public const int SQLITE_WARNING = 28;       /* Warnings from sqlite3_log() */
    public const int SQLITE_ROW = 100;       /* sqlite3_step() has another row ready */
    public const int SQLITE_DONE = 101;       /* sqlite3_step() has finished executing */
    /* end-of-error-codes */
    public const int SQLITE_INTEGER = 1;
    public const int SQLITE_FLOAT = 2;
    public const int SQLITE_BLOB = 4;
    public const int SQLITE_NULL = 5;
    public const int SQLITE_TEXT = 3;

     

    //SQLITE_API const char* sqlite3_libversion(void);

    [DllImport("SQLite3")]
    public static extern nint sqlite3_libversion();

    //    SQLITE_API int sqlite3_open(
    //  const char* filename,   /* Database filename (UTF-8) */
    //  sqlite3 **ppDb          /* OUT: SQLite db handle */
    //); 
    [DllImport("SQLite3")]
    public static extern int sqlite3_open(void* fileName, void** ppDb);




    // typedef int (* sqlite3_callback) (void*, int, char**, char**);


    //    SQLITE_API int sqlite3_exec(
    //  sqlite3*,                                  /* An open database */
    //  const char* sql,                           /* SQL to be evaluated */
    //  int (* callback) (void*, int, char**, char**),  /* Callback function */
    //  void*,                                    /* 1st argument to callback */
    //  char** errmsg                              /* Error msg written here */
    //);

    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_exec(void* pDb, void* sql, nint callback, void* argToCallback, void** pzErrMsg);

    //SQLITE_API int sqlite3_close(sqlite3*); 
    [DllImport("SQLite3")]
    public static extern int sqlite3_close(void* pDb);



    //SQLITE_API const char* sqlite3_errmsg(sqlite3 *);
    [DllImport("SQLite3")]
    public static extern nint sqlite3_errmsg(void* pDb);

    //SQLITE_API void sqlite3_free(void*);
    [DllImport("SQLite3")]
    public static unsafe extern nint sqlite3_free(void* ptr);





    //    int sqlite3_get_table(
    //  sqlite3* db,          /* An open database */
    //  const char* zSql,     /* SQL to be evaluated */
    //  char*** pazResult,    /* Results of the query */
    //  int* pnRow,           /* Number of result rows written here */
    //  int* pnColumn,        /* Number of result columns written here */
    //  char** pzErrmsg       /* Error msg written here */
    //);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_get_table(void* pDb, void* sql, void*** pazResult, int* pnRow, int* pnColumn, void** pzErrMsg);


    //    void sqlite3_free_table(char** result);
    [DllImport("SQLite3")]
    public static unsafe extern void sqlite3_free_table(void** result);

    //    //int sqlite3_prepare(
    //    sqlite3* db,            /* Database handle */
    //  const char* zSql,       /* SQL statement, UTF-8 encoded */
    //  int nByte,              /* Maximum length of zSql in bytes. */
    //  sqlite3_stmt **ppStmt,  /* OUT: Statement handle */
    //  const char** pzTail     /* OUT: Pointer to unused portion of zSql */
    //);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_prepare(void* pDb, void* sql, int nByte, void** ppStmt, void** pzTail);

    //int sqlite3_bind_blob(sqlite3_stmt*, int, const void*, int n, void (*) (void*));
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_bind_blob(void* pStmt, int sqlParameterIndex, void* pData, int nByteLen, void* callback);




    //int sqlite3_bind_double(sqlite3_stmt*, int, double);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_bind_double(void* pStmt, int sqlParameterIndex, double doubleValue);

    //int sqlite3_bind_int(sqlite3_stmt*, int, int);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_bind_int(void* pStmt, int sqlParameterIndex, int intValue);
    //int sqlite3_bind_int64(sqlite3_stmt*, int, sqlite3_int64);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_bind_int64(void* pStmt, int sqlParameterIndex, Int64 int64Value);
    //int sqlite3_bind_null(sqlite3_stmt*, int);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_bind_null(void* pStmt, int sqlParameterIndex);
    //int sqlite3_bind_text(sqlite3_stmt*, int,const char*,int,void (*) (void*));
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_bind_text(void* pStmt, int sqlParameterIndex, void* ptrUtf8String, int nByteLen, void* callback);







    //int sqlite3_step(sqlite3_stmt*);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_step(void* pStmt);
    //int sqlite3_finalize(sqlite3_stmt* pStmt);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_finalize(void* pStmt);

    //int sqlite3_reset(sqlite3_stmt* pStmt);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_reset(void* pStmt);


    //const void* sqlite3_column_blob(sqlite3_stmt *, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern void* sqlite3_column_blob(void* pStmt, int iCol);

    //double sqlite3_column_double(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern double sqlite3_column_double(void* pStmt, int iCol);

    //int sqlite3_column_int(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_column_int(void* pStmt, int iCol);
    //sqlite3_int64 sqlite3_column_int64(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern Int64 sqlite3_column_int64(void* pStmt, int iCol);
    //const unsigned char* sqlite3_column_text(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern void* sqlite3_column_text(void* pStmt, int iCol);


    //const void* sqlite3_column_text16(sqlite3_stmt *, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern void* sqlite3_column_text16(void* pStmt, int iCol);


    //sqlite3_value* sqlite3_column_value(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern void* sqlite3_column_value(void* pStmt, int iCol);

    //int sqlite3_column_bytes(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_column_bytes(void* pStmt, int iCol);

    //int sqlite3_column_bytes16(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_column_bytes16(void* pStmt, int iCol);

    //int sqlite3_column_type(sqlite3_stmt*, int iCol);
    [DllImport("SQLite3")]
    public static unsafe extern int sqlite3_column_type(void* pStmt, int iCol);

    //const char* sqlite3_column_name(sqlite3_stmt *, int N);
    [DllImport("SQLite3")]
    public static unsafe extern void* sqlite3_column_name(void* pStmt, int iCol);

    public static string? GetErrMsgFromPtr(void* pzErrMsg)
    {
        var errMsg = Marshal.PtrToStringUTF8((nint)pzErrMsg);
        Sqlite3.sqlite3_free(pzErrMsg); 
        return errMsg;
    }

    public static string? ExecuteSql(void* pDb, string sql)
    {
        string? retMsg = "OK";
        var ptrSql = Marshal.StringToCoTaskMemUTF8(sql);
        void* pErrMsg;

        var result = Sqlite3.sqlite3_exec(pDb, (void**)ptrSql, 0, (void*)0, &pErrMsg);
        Marshal.FreeCoTaskMem(ptrSql);
        if (result != Sqlite3.SQLITE_OK)
        {
            retMsg = Sqlite3.GetErrMsgFromPtr(pErrMsg);
        }
        return retMsg;
    }

}
