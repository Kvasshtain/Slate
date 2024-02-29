import { fabric } from "fabric";

let canvas = document.createElement('canvas');
canvas.id = "canvas";
document.body.append(canvas);

var fabricCanvas = new fabric.Canvas('canvas');

window.addEventListener('resize', fitFabCanvasToWindowSize, false);

function fitFabCanvasToWindowSize() {
    fabricCanvas.setHeight(window.innerHeight);
    fabricCanvas.setWidth(window.innerWidth);
    fabricCanvas.renderAll();
}

fitFabCanvasToWindowSize();

export default fabricCanvas