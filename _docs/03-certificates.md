---
title: "Certificates"
permalink: /docs/certificates/
---

Pixel Identity requires 3 certificates in all to get up and running.

- RSA certificate required for [encryption](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html) by OpenIddict.
  Certificates can be generated and self-signed locally using the .NET Core CertificateRequest API:

      using var algorithm = RSA.Create(keySizeInBits: 2048);

      var subject = new X500DistinguishedName("CN=Pixel Identity Encryption Certificate");
      var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
      request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));

      var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

      //Export the certificate as Pfx without any password
      File.WriteAllBytes("identity-encryption.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));

  Set *Identity:Certificates:EncryptionCertificatePath* and *Identity:Certificates:EncryptionCertificateKey* variables accordingly. 
  More details in [configuration guide](/docs/configuration).

- RSA certificate required for [signing](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html) by OpenIddict.

  Certificates can be generated and self-signed locally using the .NET Core CertificateRequest API:
      using var algorithm = RSA.Create(keySizeInBits: 2048);

      var subject = new X500DistinguishedName("CN=Pixel Identity Signing Certificate");
      var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
      request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

      var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));
      
      //Export the certificate as Pfx without any password
      File.WriteAllBytes("identity-signing.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));

  Set *Identity:Certificates:SigningCertificatePath* and *Identity:Certificates:SigningCertificateKey* variables accordingly.
  More details in [configuration guide](/docs/configuration).     

- HTTPS certificate required by reverse-proxy.
  While the service can be exposed directly (with Kestrel), it is recommended to use it behind a reverse proxy. See [guidance](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/when-to-use-a-reverse-proxy?view=aspnetcore-6.0) on when to use Kestrel with a reverse proxy.

  You will need to manage certificate for reverse-proxy on your own based on how you are hosting and reverse-proxy being used.
  Checkout [mkcert](https://github.com/FiloSottile/mkcert) if you are working locally on localhost. Some reverse proxy like Traefik and YARP also suppport
  integration with [Let's Encrypt](https://letsencrypt.org/) for auto generation and auto renewal of certificates.

  If you are using default docker-compose files provided as part of release, they are configured to  use [Traefik](https://traefik.io/traefik/) over localhost and requires pixel-cert.pem (certificate file) and pixel-key.pem (key file) in .certificates folder. Please see [setup guide](/docs/setup) page for more details.