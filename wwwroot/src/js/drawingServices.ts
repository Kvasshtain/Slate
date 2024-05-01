import { fabric } from "fabric";
import { HubConnection } from "@microsoft/signalr";
import { FabObjectWithId } from "./FabObjectWithId";

import { uuidv4, ucFirst, findById } from "./utilities";
import {
  fabricCanvas as fabCanvas,
  originalCanvas as origCanvas,
} from "./fabricCanvas";
import { defaultFontProperty, createTextInput } from "./textInputServices";

function makeFromDocumentBodyDropImageZone(hubConnection: HubConnection): void {
  const dropZone = document.body;

  if (dropZone) {
    //let hoverClassName = 'hover';

    dropZone.addEventListener("dragenter", (e: DragEvent) => {
      e.preventDefault();
      //dropZone.classList.add(hoverClassName);
    });

    dropZone.addEventListener("dragover", (e: DragEvent) => {
      e.preventDefault();
      //dropZone.classList.add(hoverClassName);
    });

    dropZone.addEventListener("dragleave", (e: DragEvent) => {
      e.preventDefault();
      //dropZone.classList.remove(hoverClassName);
    });

    dropZone.addEventListener("drop", (e: DragEvent) => {
      e.preventDefault();
      //dropZone.classList.remove(hoverClassName);

      let dataTransfer = e.dataTransfer;
      let target = e.target;

      if (dataTransfer === null) return;

      if (target === null) return;

      const files = Array.from(dataTransfer.files);

      if (!FileReader || files.length <= 0) return;

      let file = files[0];

      let left = e.pageX - (target as HTMLElement).offsetLeft;
      let top = e.pageY - (target as HTMLElement).offsetTop;

      let fileReader = new FileReader();

      fileReader.onload = () => {
        let result = fileReader.result as string;

        if (result === null) return;

        fabric.util.loadImage(result, (img) => {
          let oImg = new fabric.Image(img);

          oImg.set({
            left: left,
            top: top,
          });

          fabCanvas.add(oImg);
          fabCanvas.renderAll();
        });
      };

      fileReader.readAsDataURL(file);
    });
  }
}

function getAllBoardObjectsFromBackend(hubConnection: HubConnection) {
  hubConnection.invoke("GetAllBoardObjects").catch(function (err: Error) {
    return console.error(err.toString());
  });
}

function addImageManipulations(hubConnection: HubConnection) {
  makeFromDocumentBodyDropImageZone(hubConnection);
  getAllBoardObjectsFromBackend(hubConnection);

  hubConnection.on("AddObjectError", (img) => {
    //Add error handler!!!
  });

  hubConnection.on("AddObjectOnCanvas", (obj) => {
    let id = obj.id;
    let canvasObj = JSON.parse(obj.data);
    let left = obj.left;
    let top = obj.top;
    let scaleX = obj.scaleX;
    let scaleY = obj.scaleY;
    let angle = obj.angle;

    fabric.util.enlivenObjects(
      [canvasObj],
      function (objects: any[]) {
        var origRenderOnAddRemove = fabCanvas.renderOnAddRemove;
        fabCanvas.renderOnAddRemove = false;

        objects.forEach(function (o: any) {
          o.set({
            id: id,
            left: left,
            top: top,
            scaleX: scaleX,
            scaleY: scaleY,
            angle: angle,
          });

          fabCanvas.add(o);
        });

        fabCanvas.renderOnAddRemove = origRenderOnAddRemove;
        fabCanvas.renderAll();
      },
      ""
    );

    fabCanvas.renderAll();
  });

  hubConnection.on("MoveObjectOnCanvas", (payload) => {
    let id = payload.id;
    let left = payload.left;
    let top = payload.top;

    let obj = findById(fabCanvas, id);

    if (!obj) return;

    obj.set({
      left: left,
      top: top,
    });

    fabCanvas.renderAll();
  });

  hubConnection.on("ScaleObjectOnCanvas", (payload) => {
    let id = payload.id;
    let left = payload.left;
    let top = payload.top;
    let scaleX = payload.scaleX;
    let scaleY = payload.scaleY;

    let obj = findById(fabCanvas, id);

    if (!obj) return;

    obj.set({
      left: left,
      top: top,
      scaleX: scaleX,
      scaleY: scaleY,
    });

    fabCanvas.renderAll();
  });

  hubConnection.on("RotateObjectOnCanvas", (payload) => {
    let id = payload.id;
    let angle = payload.angle;

    let obj = findById(fabCanvas, id);

    if (!obj) return;

    obj.set({
      angle: angle,
    });

    fabCanvas.renderAll();
  });

  hubConnection.start().catch(function (err) {
    return console.error(err.toString());
  });

  //Type text

  fabCanvas.on("mouse:down", function (options) {
    if (options.e) {
      let evt = options.e;
      let textX = evt.pageX;
      let textY = evt.pageY;
      let value = "";

      createTextInput(
        textX,
        textY,
        value,
        defaultFontProperty,
        function (textArea: HTMLTextAreaElement) {
          let textStyle = textArea.style;

          let val = textArea.value;

          if (val !== null && typeof val !== "undefined") {
            val = val.trim();
          }

          if (!val) {
            return;
          }

          var text = new fabric.Text(val, {
            left: textX,
            top: textY,
            fill: textStyle.color,
            fontSize: parseInt(textStyle.fontSize),
            fontWeight: textStyle.fontWeight,
            //textDecoration: textStyle.textDecoration, //!!!!!!!!!!!!!!!
            shadow: textStyle.boxShadow,
            fontStyle: textStyle.fontStyle as
              | ""
              | "normal"
              | "italic"
              | "oblique"
              | undefined,
            fontFamily: textStyle.fontFamily,
            stroke: textStyle.stroke,
            strokeWidth: +textStyle.strokeWidth,
            textAlign: textStyle.textAlign,
            lineHeight: +textStyle.lineHeight,
            textBackgroundColor: textStyle.backgroundColor,
          });

          fabCanvas.add(text);
          fabCanvas.renderAll();
        }
      );
    }
  });

  fabCanvas.on("object:added", (evt) => {
    let target = evt.target as FabObjectWithId;

    if (target === undefined) return;

    if (target.id === undefined) {
      let id = uuidv4();
      let left = target.left;
      let top = target.top;
      let scaleX = target.scaleX;
      let scaleY = target.scaleY;

      let jsonData = JSON.stringify(target);

      fabCanvas.remove(target);

      hubConnection
        .invoke("AddObject", {
          Id: id,
          Data: jsonData,
          Left: left,
          Top: top,
          ScaleX: scaleX,
          ScaleY: scaleY,
        })
        .catch(function (err) {
          return console.error(err.toString());
        });

      //fabCanvas.isDrawingMode = false; //!!!!
    }
    //fabCanvas.isDrawingMode = false; //!!!!
  });

  fabCanvas.on("object:modified", (evt) => {
    let action = evt.action as string;

    if (action === null) return;

    let method = ucFirst(action);

    var modifiedObject = evt.target as FabObjectWithId;

    if (modifiedObject === undefined || modifiedObject === null) return;

    let id = modifiedObject.get("id");
    let left = modifiedObject.get("left");
    let top = modifiedObject.get("top");
    let scaleX = modifiedObject.get("scaleX");
    let scaleY = modifiedObject.get("scaleY");
    let angle = modifiedObject.get("angle");

    let payload;

    switch (method) {
      case "Drag":
        payload = {
          Id: id,
          Left: left,
          Top: top,
        };
        break;
      case "Scale":
      case "ScaleX":
      case "ScaleY":
        method = "Scale";
        payload = {
          Id: id,
          Left: left,
          Top: top,
          ScaleX: scaleX,
          ScaleY: scaleY,
        };
        break;
      case "Rotate":
        payload = {
          Id: id,
          Angle: angle,
        };
        break;
      default:
        return;
    }

    hubConnection.invoke(method, payload).catch(function (err) {
      return console.error(err.toString());
    });
  });
}

export { addImageManipulations };
