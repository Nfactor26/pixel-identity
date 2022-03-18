export function generateQrCode() {
    const uri = document.getElementById("qrCodeData").getAttribute('data-url');
    if (uri == null) {
        return;
    }
    new QRCode(document.getElementById("qrCode"),
        {
            text: uri,
            width: 150,
            height: 150
        });
}