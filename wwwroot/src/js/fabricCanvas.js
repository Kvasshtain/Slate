import { fabric } from "fabric";

let originalCanvas = document.createElement('canvas');
originalCanvas.id = "canvas";
document.body.append(originalCanvas);

var fabricCanvas = new fabric.Canvas('canvas');

//fabricCanvas.isDrawingMode = true;

window.addEventListener('resize', fitFabCanvasToWindowSize, false);

function fitFabCanvasToWindowSize() {
    fabricCanvas.setHeight(window.innerHeight);
    fabricCanvas.setWidth(window.innerWidth);
    fabricCanvas.renderAll();
}

fitFabCanvasToWindowSize();

export {fabricCanvas, originalCanvas};