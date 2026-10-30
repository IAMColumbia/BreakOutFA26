window.breakout = {
    start(instance) {
        const canvas = document.getElementById("theCanvas");
        canvas.width = 800;
        canvas.height = 480;
        canvas.addEventListener("pointerdown", () => canvas.focus());
        canvas.addEventListener("contextmenu", event => event.preventDefault());
        window.addEventListener("keydown", event => {
            if (document.activeElement === canvas && ["ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown", " "].includes(event.key)) {
                event.preventDefault();
            }
        });
        canvas.focus();
        function tick() {
            try {
                instance.invokeMethod("TickDotNet");
                window.requestAnimationFrame(tick);
            } catch (error) {
                console.error(error);
                document.getElementById("blazor-error-ui").style.display = "block";
            }
        }
        window.requestAnimationFrame(tick);
    }
};
