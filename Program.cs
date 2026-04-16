using System.Text.Json;
using System.Net.Http.Headers;

HttpClient client = new HttpClient();
HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://resultadoelectoral.onpe.gob.pe/presentacion-backend/resumen-general/participantes?idEleccion=10&tipoFiltro=eleccion");

request.Headers.Add("accept", "*/*");
request.Headers.Add("accept-language", "es-419,es;q=0.9,es-ES;q=0.8,en;q=0.7,en-GB;q=0.6,en-US;q=0.5");
request.Headers.Add("priority", "u=1, i");
request.Headers.Add("referer", "https://resultadoelectoral.onpe.gob.pe/main/resumen");
request.Headers.Add("sec-ch-ua", "\"Microsoft Edge\";v=\"147\", \"Not.A/Brand\";v=\"8\", \"Chromium\";v=\"147\"");
request.Headers.Add("sec-ch-ua-mobile", "?1");
request.Headers.Add("sec-ch-ua-platform", "\"Android\"");
request.Headers.Add("sec-fetch-dest", "empty");
request.Headers.Add("sec-fetch-mode", "cors");
request.Headers.Add("sec-fetch-site", "same-origin");
request.Headers.Add("user-agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Mobile Safari/537.36 Edg/147.0.0.0");

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

    //Console.WriteLine($"Votos Juntos por el Perú: {votosPartido10:N0}");
    //Console.WriteLine($"Votos Renovación Popular: {votosPartido35:N0}");
    //Console.WriteLine($"-----------------------------");
    //Console.WriteLine($"Diferencia: {diferencia:N0} votos.");

    string template = File.ReadAllText("template.html");

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