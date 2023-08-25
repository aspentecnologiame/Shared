import { Subject, Observable } from 'rxjs';
import { AccessDataModel } from './../models';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  private readonly _tokenRequest = new Subject<boolean>();

  readonly localStorageKey = 'currentUser';

  obterDadosDeAcesso(): AccessDataModel {
    if (localStorage.getItem(this.localStorageKey) === null) {
      return null;
    }

    return JSON.parse(localStorage.getItem(this.localStorageKey)) as AccessDataModel;
  }

  obterToken(): string {
    this._tokenRequest.next(true);

    const accessDataModel = this.obterDadosDeAcesso();

    if (!accessDataModel) {
      return null;
    }

    return accessDataModel.accessToken;
  }

  obterTokenEvent(): Observable<boolean> {
    return this._tokenRequest.asObservable();
  }

}
