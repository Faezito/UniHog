using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Z3.DataAccess.Externo;

public interface IAmazonS3Servicos
{
    Task<string> UploadImagem(IFormFile arquivo);
}

public class AmazonS3Servicos : IAmazonS3Servicos
{
    private readonly IConfiguration _config;
    private readonly IAPIsDataAccess _apis;

    public AmazonS3Servicos(IConfiguration config, IAPIsDataAccess apis)
    {
        _config = config;
        _apis = apis;
    }

    public async Task<string> UploadImagem(IFormFile arquivo)
    {
        var API = await _apis.Obter(25, 103);

        var accessKey = API.Token;
        var secretKey = API.Senha;
        var region = API.Modelo;
        var bucketName = API.Usuario;

        var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));

        var nomeArquivo = Guid.NewGuid() + Path.GetExtension(arquivo.FileName);

        using var stream = arquivo.OpenReadStream();

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            Key = $"uploads/{nomeArquivo}",
            BucketName = bucketName,
            ContentType = arquivo.ContentType,
            CannedACL = S3CannedACL.PublicRead // deixa público
        };

        var transferUtility = new TransferUtility(s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        return $"https://{bucketName}.s3.{region}.amazonaws.com/uploads/{nomeArquivo}";
    }
}
