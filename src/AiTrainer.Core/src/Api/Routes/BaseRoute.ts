import { Application } from "express";

export default abstract class BaseRoute {
  protected readonly _app: Application;
  constructor(app: Application) {
    this._app = app;
  }

  public abstract InvokeRoutes(): void;
}
