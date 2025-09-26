namespace Persistence.Services;

internal class StorageService : IStorageService
{
    public byte[] DownloadFile(string fileName, string bucketName, byte[] _key, byte[] _iv)
    {
        throw new NotImplementedException();
    }

    public (byte[], byte[]) UploadFileAsync(byte[] binaryFile, string fileName, string bucketName)
    {
        throw new NotImplementedException();
    }
}
