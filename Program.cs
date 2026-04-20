using System.Text.Json;
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

HttpClient client2 = new HttpClient();
HttpRequestMessage request2 = new HttpRequestMessage(HttpMethod.Get, "https://resultadoelectoral.onpe.gob.pe/presentacion-backend/resumen-general/totales?idEleccion=10&tipoFiltro=eleccion");

request2.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:149.0) Gecko/20100101 Firefox/149.0");
request2.Headers.Add("Accept", "*/*");
request2.Headers.Add("Accept-Language", "es-ES,es;q=0.9,en-US;q=0.8,en;q=0.7");
// request.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
request2.Headers.Add("Referer", "https://resultadoelectoral.onpe.gob.pe/main/resumen");
request2.Headers.Add("DNT", "1");
request2.Headers.Add("Connection", "keep-alive");
request2.Headers.Add("Sec-Fetch-Dest", "empty");
request2.Headers.Add("Sec-Fetch-Mode", "cors");
request2.Headers.Add("Sec-Fetch-Site", "same-origin");
request2.Headers.Add("TE", "trailers");

//VOTOS
request.Content = new StringContent("");
request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

HttpResponseMessage response = await client.SendAsync(request);
response.EnsureSuccessStatusCode();
string responseBody = await response.Content.ReadAsStringAsync();
var resultado = JsonSerializer.Deserialize<Root>(responseBody);
/**********************/

//HORA ACTUALIZACION ONPE
request2.Content = new StringContent("");
request2.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

HttpResponseMessage response2 = await client2.SendAsync(request2);
response2.EnsureSuccessStatusCode();
string responseHeaderDate = await response2.Content.ReadAsStringAsync();
var resultado2 = JsonSerializer.Deserialize<CabeceraOnpe>(responseHeaderDate);
/**********************/


if (resultado?.data != null && resultado2?.data != null)
{
    var votosPartido10 = resultado.data.FirstOrDefault(x => x.codigoAgrupacionPolitica == 10)?.totalVotosValidos ?? 0;
    var votosPartido35 = resultado.data.FirstOrDefault(x => x.codigoAgrupacionPolitica == 35)?.totalVotosValidos ?? 0;

    long diferencia = votosPartido10 - votosPartido35;

    DateTime fechaActualizacion = DateTimeOffset.FromUnixTimeMilliseconds(resultado2.data.fechaActualizacion).DateTime.AddHours(-5);

    string rutaRaiz = Directory.GetCurrentDirectory();
    string templatePath = Path.Combine(rutaRaiz, "template.html");
    string outputPath = Path.Combine(rutaRaiz, "index.html");

    if (File.Exists(templatePath))
    {
        string template = File.ReadAllText(templatePath);

        string htmlFinal = template
            .Replace("{{PORCENTAJE}}", resultado2.data.actasContabilizadas.ToString())
            .Replace("{{DIFERENCIA}}", diferencia.ToString("N0"))
            .Replace("{{VOTOS10}}", votosPartido10.ToString("N0"))
            .Replace("{{VOTOS35}}", votosPartido35.ToString("N0"))
            .Replace("{{FECHA}}", fechaActualizacion.ToString("dd/MM/yyyy HH:mm:ss"));

        File.WriteAllText(outputPath, htmlFinal);
        Console.WriteLine("¡HTML restaurado y actualizado con éxito!");
    }
}

public class CabeceraOnpe
{
    public DataContainer data { get; set; }
}

public class DataContainer
{
    public decimal actasContabilizadas { get; set; }
    public long fechaActualizacion { get; set; }
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