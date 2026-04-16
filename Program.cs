using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;

HttpClient client = new HttpClient();

HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://resultadoelectoral.onpe.gob.pe/presentacion-backend/resumen-general/participantes?idEleccion=10&tipoFiltro=eleccion");

request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:149.0) Gecko/20100101 Firefox/149.0");
request.Headers.Add("Accept", "*/*");
request.Headers.Add("Accept-Language", "es-ES,es;q=0.9,en-US;q=0.8,en;q=0.7");
// request.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
request.Headers.Add("Referer", "https://resultadoelectoral.onpe.gob.pe/main/resumen");
request.Headers.Add("DNT", "1");
request.Headers.Add("Connection", "keep-alive");
request.Headers.Add("Sec-Fetch-Dest", "empty");
request.Headers.Add("Sec-Fetch-Mode", "cors");
request.Headers.Add("Sec-Fetch-Site", "same-origin");
request.Headers.Add("TE", "trailers");

request.Content = new StringContent("");
request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

HttpResponseMessage response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();
string responseBody = await response.Content.ReadAsStringAsync();

var resultado = JsonSerializer.Deserialize<Root>(responseBody);

if (resultado?.data != null)
{
    var votosPartido10 = resultado.data.FirstOrDefault(x => x.codigoAgrupacionPolitica == 10)?.totalVotosValidos ?? 0;
    var votosPartido35 = resultado.data.FirstOrDefault(x => x.codigoAgrupacionPolitica == 35)?.totalVotosValidos ?? 0;

    long diferencia = votosPartido10 - votosPartido35;

    Console.WriteLine($"Votos Juntos por el Perú: {votosPartido10:N0}");
    Console.WriteLine($"Votos Renovación Popular: {votosPartido35:N0}");
    Console.WriteLine($"-----------------------------");
    Console.WriteLine($"Diferencia: {diferencia:N0} votos.");

    string template = File.ReadAllText("template.html");

    //DateTime horaPeru = DateTime.UtcNow.AddHours(-5);

    string htmlFinal = template
        .Replace("{{DIFERENCIA}}", diferencia.ToString("N0"))
        .Replace("{{VOTOS10}}", votosPartido10.ToString("N0"))
        .Replace("{{VOTOS35}}", votosPartido35.ToString("N0"))
        .Replace("{{FECHA}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

    File.WriteAllText("index.html", htmlFinal);

    //Console.WriteLine("Archivo HTML actualizado con éxito.");
}
else
{
    //Console.WriteLine("No se pudo obtener la data.");
}

public class Agrupacion
{
    public int codigoAgrupacionPolitica { get; set; }
    public long totalVotosValidos { get; set; }
}

public class Root
{
    public List<Agrupacion> data { get; set; }
}