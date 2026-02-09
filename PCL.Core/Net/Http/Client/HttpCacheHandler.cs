using System.IO;
using System.Net.Http;
using Microsoft.Data.Sqlite;
using PCL.Core.IO.Storage;

namespace PCL.Core.Net.Http.Client;

/// <summary>
/// 
/// </summary>
public class HttpCacheHandler:DelegatingHandler
{

    private SqliteConnection? _database;
    private HashStorage? _stpre;
    
    public HttpCacheHandler(HttpCachePolicy policy)
    {
        if (!Directory.Exists(policy.Path)) Directory.CreateDirectory(policy.Path);
        var path = Path.GetPathRoot(Path.GetFullPath(policy.Path));
        if (path is not null && policy.MaxSize != -1L)
        {
            var drive = new DriveInfo(path);
            if (drive.AvailableFreeSpace < policy.MaxSize) 
                throw new HttpCacheException($"{drive.Name} haven't space (require {policy.MaxSize} Bytes, current is {drive.AvailableFreeSpace} Bytes.)");
        }

        var dbPath = Path.Combine(policy.Path, "cache.db");
        var dirInfo = new DirectoryInfo(policy.Path);
        var subFiles = dirInfo.GetFiles(policy.Path);
        switch (subFiles.Length)
        {
            case 0:
                _database = new SqliteConnection("");
                break;
            default:
                if (!policy.RemoveExistFile) throw new HttpCacheException("Directory not empty");
                dirInfo.Delete(true);
                break;
        }
    }

    private void _HandleCacheContorl(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return;
        var etag = response.Headers.ETag;
        var vary = response.Headers.Vary;
        var nonValidated = response.Headers.NonValidated;
        var control = response.Headers.CacheControl;
        if (control is
            {
                NoCache: var noCache,
                NoStore: var noStore,
                MustRevalidate: var mustRevalidated,
                MaxAge: var maxAge,
                
            })
        {
            
        }
    }
}
