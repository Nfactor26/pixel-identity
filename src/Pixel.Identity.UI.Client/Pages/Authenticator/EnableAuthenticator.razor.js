export function generateQrCode() {
    const uri = document.getElementById("qrCodeData").getAttribute('data-url');
    const title = document.getElementById("qrCode").getAttribute('title');
    if (uri == null || title != null) {
        return;
    }
    new QRCode(document.getElementById("qrCode"),
        {
            text: uri,
            width: 150,
            height: 150
        });
}