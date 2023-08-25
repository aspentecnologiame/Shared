import { ErrorHandler, Injectable } from "@angular/core";
import { AppInsightsService } from "./app-insights.service";

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService extends ErrorHandler {

  constructor(private appInsightsService: AppInsightsService) {
    super();
  }

  handleError(error: Error) {
    this.appInsightsService.logException(error);
  }
}
