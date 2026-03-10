using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Z3.DataAccess.Externo;

public interface ICloudinaryServicos
{
    Task<(string? url, string? publicId)> UploadFotoPerfil(IFormFile file, string cpf);
    Task<(string? url, string? publicId)> UploadNotaFiscal(IFormFile file, string id);
    Task<bool> DeletarImagem(string publicId);
}
public class CloudinaryServicos : ICloudinaryServicos
{
    private readonly IAPIsDataAccess _api;
    private readonly IConfiguration _config;
    private Cloudinary _cloudinary;
    private const long TAMANHO_MAXIMO = 2 * 1024 * 1024;

    public CloudinaryServicos(IConfiguration config, IAPIsDataAccess api)
    {
        _api = api;
        _config = config;
    }

    public async Task<(string? url, string? publicId)> UploadFotoPerfil(IFormFile file, string tel)
    {
        await InicializarCloudinary();

        if (file == null || file.Length == 0)
            return (null, null);

        if (file.Length > TAMANHO_MAXIMO)
            throw new Exception("A imagem não pode ser maior que 2MB.");

        using var stream = file.OpenReadStream();
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"fotoperfil_{tel}_{DateTime.Now.Ticks}";

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = fileName,
            Folder = $"BOPE/usuarios/{tel}" // pasta no Cloudinary
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return (result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task<bool> DeletarImagem(string publicId)
    {
        await InicializarCloudinary(); // seu método de init

        var deletionParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (result.Result == "ok")
            return true;
        if (result.Result == "not found")
            return true;

        throw new Exception($"Erro ao deletar imagem: {result.Error?.Message}");
    }


    private async Task InicializarCloudinary()
    {
        var api = await _api.Obter(null, 101);

        var cloudName = api.Usuario;
        var apiKey = api.Token; ;
        var apiSecret = api.Senha;

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<(string? url, string? publicId)> UploadNotaFiscal(IFormFile file, string id)
    {
        await InicializarCloudinary();

        if (file == null || file.Length == 0)
            return (null, null);

        using var stream = file.OpenReadStream();
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"NF_{DateTime.Now.Ticks}_{id}";

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            PublicId = fileName,
            Folder = $"BOPE/NF/{DateTime.Now.ToString("yyyy-MM-dd")}" // pasta no Cloudinary
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        return (result.SecureUrl.ToString(), result.PublicId);
    }
}
