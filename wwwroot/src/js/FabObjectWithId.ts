import { fabric } from "fabric";

class FabObjectWithId extends fabric.Object {
    
    private _id!: string;

    public get id(): string {
        return this._id;
    }

    public set id(str: string) {
        this._id = str;
    }
}

export {FabObjectWithId};