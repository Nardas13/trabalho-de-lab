const canvas = document.getElementById("sparkline");
if (canvas) {
    const ctx = canvas.getContext("2d");

    const values = [2, 3, 1, 4, 2, 5, 3, 4];

    ctx.strokeStyle = "rgba(15,23,42,0.4)";
    ctx.lineWidth = 2;

    ctx.beginPath();
    ctx.moveTo(0, 40 - values[0] * 6);

    values.forEach((v, i) => {
        ctx.lineTo(i * 25, 40 - v * 6);
    });

    ctx.stroke();
}
