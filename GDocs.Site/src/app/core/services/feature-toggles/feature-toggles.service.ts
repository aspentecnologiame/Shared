import { Injectable } from "@angular/core";
import { AppConfig } from "../../../app.config";
import { FeatureToggles } from "../../enums/feature-toggles-enum";


@Injectable({
  providedIn: 'root'
})
export class FeatureTogglesService {
  public verificarSeAFeatureEstaAtiva(feature: FeatureToggles): boolean {
    if (AppConfig.settings.featureToggles === null || AppConfig.settings.featureToggles === undefined) {
      return false;
    }

    return AppConfig.settings.featureToggles.filter(e => e.nome === feature.toString() && e.ativo).length > 0;
  }
}
