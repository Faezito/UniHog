using UniHog.Libraries.Login;
using UniHog.Libraries.Sessao;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Serilog;
using System.Data.Common;
using System.Globalization;
using Z1.Model;
using Z2.Services.Externo;
using Z2.Servicos;
using Z2.Servicos.Externo;
using Z2.Servicos.Lib;
using Z3.DataAccess;
using Z3.DataAccess.Database;
using Z3.DataAccess.Externo;

var builder = WebApplication.CreateBuilder(args);
var culture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

// Autenticação única
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Home/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();


//.AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["GoogleAuth:ClientId"];
//    googleOptions.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
//    googleOptions.CallbackPath = "/signin-google";

//    // Claims extras
//    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");
//    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
//    googleOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
//});

// Data Protection persistente
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys")) // LOCAL NO SERVIDOR
    .SetApplicationName("UniHog");

// Sessão
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//builder.Services.AddHttpClient<IViaCepServicos, ViaCepServicos>(client =>
//{
//    client.BaseAddress = new Uri("https://viacep.com.br/ws/");
//    client.Timeout = TimeSpan.FromSeconds(10);
//});

builder.Services.AddHttpClient();

builder.Services.AddHostedService<ManterAPIOnlineServicos>();

builder.Services.AddScoped<IDapper, DapperDatabase>(x =>
{
    string strProv = "Microsoft.Data.SqlClient";
    string strConn = builder.Configuration.GetValue<string>("ConnectionStrings:DB");

    if (string.IsNullOrWhiteSpace(strConn))
    {
        throw new InvalidOperationException(
            "A variável de ambiente para o banco de dados não está definida."
        );
    }

    DbProviderFactories.RegisterFactory(strProv, Microsoft.Data.SqlClient.SqlClientFactory.Instance);
    return new DapperDatabase(strProv, strConn);
});


builder.Services.AddScoped<IAPIsDataAccess, APIsDataAccess>();
builder.Services.AddScoped<IAPIsServicos, APIsServicos>();
builder.Services.AddScoped<IGeminiServicos, GeminiServicos>();
builder.Services.AddScoped<IUsuarioServicos, UsuarioServicos>();
builder.Services.AddScoped<IUsuarioDataAccess, UsuarioDataAccess>();
builder.Services.AddScoped<IEnderecoDataAccess, EnderecoDataAccess>();
builder.Services.AddScoped<IEnderecoServicos, EnderecoServicos>();
builder.Services.AddScoped<IEmailServicos, EmailServicos>();
builder.Services.AddScoped<IEmailDataAccess, EmailDataAccess>();
builder.Services.AddScoped<IAmazonS3Servicos, AmazonS3Servicos>();
builder.Services.AddScoped<ICloudinaryServicos, CloudinaryServicos>();
builder.Services.AddScoped<IEstudoServicos, EstudoServicos>();
builder.Services.AddScoped<IEstudoDataAccess, EstudoDataAccess>();
builder.Services.AddScoped<IProjetoServicos, ProjetoServicos>();
builder.Services.AddScoped<IProjetoDataAccess, ProjetoDataAccess>();
builder.Services.AddScoped<IProfessorServicos, ProfessorServicos>();
builder.Services.AddScoped<IProfessorDataAccess, ProfessorDataAccess>();
builder.Services.AddScoped<IAtendimentoDataAccess, AtendimentoDataAccess>();
builder.Services.AddScoped<IAtendimentoServicos, AtendimentoServicos>();
builder.Services.AddScoped<IInteressadoServicos, InteressadoServicos>();
builder.Services.AddScoped<IInteressadoDA, InteressadoDA>();
builder.Services.AddScoped<IEspecialidadeServicos, EspecialidadeServicos>();
builder.Services.AddScoped<IEspecialidadeDataAccess, EspecialidadeDataAccess>();
builder.Services.AddScoped<IPessoaJuridicaServicos, PessoaJuridicaServicos>();
builder.Services.AddScoped<IPessoaJuridicaDataAccess, PessoaJuridicaDataAccess>();
builder.Services.AddScoped<IMovimentacaoFinanceiraServicos, MovimentacaoFinanceiraServicos>();
builder.Services.AddScoped<IMovimentacaoFinanceiraDataAccess, MovimentacaoFinanceiraDataAccess>();
builder.Services.AddScoped<ICestaBasicaServicos, CestaBasicaServicos>();
builder.Services.AddScoped<ICestaBasicaDataAccess, CestaBasicaDataAccess>();
builder.Services.AddScoped<IViaCepServicos, ViaCepServicos>();
builder.Services.AddScoped<IDevToolsServicos, DevToolsServicos>();
builder.Services.AddScoped<IClientFactoryPost, ClientFactoryPost>();
builder.Services.AddScoped<IClientFactoryGet, ClientFactoryGet>();
builder.Services.AddScoped<IClientFactoryDelete, ClientFactoryDelete>();
builder.Services.AddScoped<IChamadoServicos, ChamadoServicos>();
builder.Services.AddScoped<IChamadoDataAccess, ChamadoDataAccess>();
builder.Services.AddScoped<ILimparTabelas, LimparTabelas>();


//// Configurações de sessão

builder.Services.AddMemoryCache(); // serve para guardar os dados na memória
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<Sessao>();
builder.Services.AddScoped<LoginUsuario>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseCookiePolicy();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();