using Google.Cloud.Storage.V1;
using SW.PrimitiveTypes;

namespace SW.CloudFiles.GC;

// {
//   "type": "service_account",
//   "project_id": "mudaraba-platform",
//   "private_key_id": "1cc48618005481472503dbe8428c0ee078d03451",
//   "private_key": "-----BEGIN PRIVATE KEY-----\nMIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQDXCMiXJED3p/V5\nGeC5qREFbMsXbQ/TFu9G+E4BnrjhrebKvGSlVssRVYt9gicXUW9M/Cqt6/QbWahs\nH6ztu3FcC4sQyESztDdvltEvjd0AwawnSjh40jHBkpsNkDuXe7Sv2fj84b6YTOjC\nn5dIrkVFX5aEufzswbc6clds9S8uh0k34lEyeAnmAK6f05kxHmQvlOV6145ChC7M\naxgooW8iOuLLZ5Z0/41v6x9ZFvKv7ZYeP8TsurEV9l2viPxCzecsuk9P/GNDoslQ\nuQuMzBZdeX9daOd+57ikRpxz1w3tDkEhZ26TwTJk63Bd6jpdB1TiRXoQ1zs44EIO\npDgTqIVbAgMBAAECggEAVGjwRNRIZG8cHwOXgYnqUpTYTEPVmGlCWDuUc3JY5M4c\n8KxvXa/qWs2XyhbTPYYMCM5b+pXK5wmU4Yy1l0SjRRyK5HWY8mnl8Pnk96PagzeD\nWpD6BgOM4I2qK/LYUaiPzw/je3EqHiLio7N98VvneUoCjetsa4vHXzwPMYg1ky9z\nuFec5g5NnEhIupU+Vouzmq+yu9EOUScTUHQR6TyDti2K8cOpE7Rg8nbzPVtMI3HT\nZoGxs8tLjVVzcTYmUfnKDgaHkIvQ2tJavs6aurDi/yj3FhNFGODYCwDT1uNi4FVj\nQRHrl4xWlT5vS2IrsEVpTK5SBgf+xPVBl+ttcnjwcQKBgQDxlhy++aXFqzyfYdEF\nBIkC0w3CWoG/zB/fGOzmKhy8TPsa8X3F8AuF6arEdHRUPumk//svxMuxxTOetEfU\nTyferqFHDNchx7Ls4CqOZg1lKL/LAWZ2uq/nTaMzn91gRh8AwPupGyWrDe6wfJfp\nKQyknluviHK5hsuie0h0idUYxwKBgQDj3SBVAa5EXPBPxqNMbevptNwvNjCRhTR7\naSUhSIy6xFwXEXlMxi166HgQVkVDcSKaPhSTqx4xC19JrBaXGnnzKM/L/PoiT6mr\nv2o7YdFnOsaohvl5EGxt+BM19hNBOoFvo7K3L856AnQEYoLT5TdGTk5PMkGhWazo\nhlotjrzizQKBgQDS7nMY15ZCuZkLu/co/2W1Pptj12w0D0DApN2qtJg0XK5ePv3G\nxij5eiSstNUg/XE4rHwfoB8NjxXb+qJAoAA6sJSGGZL4noj9w1fEzxvg+CxFTmqt\nPaD6PtJA79L1DSRinxzb8KT57gc6tv5YCIxOaym4YFVy4VuI+UUw42tY3wKBgCsH\nnwtufdI7GsZ15HCScXJ4zPu9Z/6TAQ4tFO3sHWdHnXxduGJfKkeDjujG4d2Sh85I\n2unn7pOkaiIndTyjq1PX6SXEaBgCoy/jvdPo7PpphwpcMtBB4bgCmN4f0hMHUVob\nt6wQQxWXUQFi+QG6z21fbDpYazxlCtn+RRlFUzYhAoGBALH5NlgHtLWQjQkxhDAm\nyKmRXvDpEORgvn53WjS2kkTJ/JoCROnqrHo/GXxEOR4FcHadL3CNuDKa8btYNhuM\nd3InolBwC2BSrEvcaGHoy6lhjj8DWrAiSDaJcVJrShGXUc72MTKCxifp2VrByi9p\nGblIMAR9rtjt2QqVZnPIvj8k\n-----END PRIVATE KEY-----\n",
//   "client_email": "sa-bucket-dev@mudaraba-platform.iam.gserviceaccount.com",
//   "client_id": "101052727388648379084",
//   "auth_uri": "https://accounts.google.com/o/oauth2/auth",
//   "token_uri": "https://oauth2.googleapis.com/token",
//   "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
//   "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/sa-bucket-dev%40mudaraba-platform.iam.gserviceaccount.com",
//   "universe_domain": "googleapis.com"
// }

public class GoogleCloudFilesOptions: CloudFilesOptions
{
    public string ProjectId { get; set; }
    public string PrivateKeyId { get; set; }
    public string PrivateKey { get; set; }
    public string ClientEmail { get; set; }
    public string ClientId { get; set; }
    public string AuthUri { get; set; } = "https://accounts.google.com/o/oauth2/auth";
    public string TokenUri { get; set; } = "https://oauth2.googleapis.com/token";
    public string AuthProviderX509CertUrl { get; set; } ="https://www.googleapis.com/oauth2/v1/certs";
    public string ClientX509CertUrl { get; set; }
    public string UniverseDomain { get; set; } = "googleapis.com";
}