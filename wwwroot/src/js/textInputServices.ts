import { IFontProperties } from "./IFontProperties";

const textAreaId = "textArea";
const hiddenDivId = "hiddenDiv";

const defaultFontProperty: IFontProperties = {
  fontSize: 16 + "px",
  fontWeight: "normal",
  color: "red",
  textDecoration: "rgba(0,0,0,0) 0 0 0px",
  shadow: "rgba(0,0,0,0) 0px 0px 0px",
  fontStyle: "normal",
  fontFamily: "Times New Roman",
  stroke: "",
  strokeWidth: "",
  textAlign: "left",
  lineHeight: 1,
  textBackgroundColor: "rgba(0,0,0,0)",
};

let createTextArea = (
  textX: number,
  textY: number,
  value: string,
  fontProperty: IFontProperties
) => {
  let textArea = document.createElement("textarea");
  textArea.id = textAreaId;

  textArea.value = value;

  let style = textArea.style;

  //transfer to css
  style.position = "absolute";
  style.overflow = "hidden";
  style.minWidth = "100px";
  style.minHeight = "30px";
  style.padding = "5px";
  style.whiteSpace = "nowrap";

  style.left = textX.toString() + "px";
  style.top = textY.toString() + "px";

  style.fontSize = fontProperty.fontSize;
  style.fontWeight = fontProperty.fontWeight;
  style.color = fontProperty.color;
  style.textDecoration = fontProperty.textDecoration;
  //style.shadow = fontProperty.shadow; //!!!!!!!!!!!!
  style.fontStyle = fontProperty.fontStyle;
  style.fontFamily = fontProperty.fontFamily;
  style.stroke = fontProperty.stroke;
  style.strokeWidth = fontProperty.strokeWidth;
  style.textAlign = fontProperty.textAlign;
  style.lineHeight = fontProperty.lineHeight.toString();
  //style.textBackgroundColor = fontProperty.textBackgroundColor; //!!!!!!!!!!!

  setTimeout(function () {
    textArea.focus();
  }, 0);

  return textArea;
};

let createAuxiliaryHiddenDiv = (textArea: HTMLTextAreaElement) => {
  let hiddenDiv = document.createElement("div");
  hiddenDiv.id = hiddenDivId;

  let style = hiddenDiv.style;

  //transfer to css
  style.position = "absolute";
  style.zIndex = "-1000";

  style.left = textArea.style.left;
  style.top = textArea.style.top;
  style.whiteSpace = textArea.style.whiteSpace;
  style.minWidth = textArea.style.minWidth;
  style.minHeight = textArea.style.minHeight;
  style.padding = textArea.style.padding;
  style.fontSize = textArea.style.fontSize;
  style.fontWeight = textArea.style.fontWeight;
  style.fontFamily = textArea.style.fontFamily;
  style.lineHeight = textArea.style.lineHeight;

  return hiddenDiv;
};

type TextEditingEndHandler = (textArea: HTMLTextAreaElement) => void;

function createTextInput(
  textX: number,
  textY: number,
  value: string,
  fontProperty: IFontProperties,
  textEditingEndHandler: TextEditingEndHandler
) {
  let textArea = createTextArea(textX, textY, value, fontProperty);
  let hiddenDiv = createAuxiliaryHiddenDiv(textArea);

  textArea.addEventListener("keyup", function () {
    let content = textArea.value.replace(/\n/g, "<br>");
    hiddenDiv.innerHTML = content;
    let clientRect = hiddenDiv.getBoundingClientRect();
    textArea.style.height = clientRect.height + "px";
    textArea.style.width = clientRect.width + "px";
  });

  let onblur = () => {
    textEditingEndHandler(textArea);
    textArea.remove();
    hiddenDiv.remove();
  };

  textArea.addEventListener("blur", onblur, { once: true });

  document.body.append(textArea);
  document.body.append(hiddenDiv);
}

export { defaultFontProperty, createTextInput };
