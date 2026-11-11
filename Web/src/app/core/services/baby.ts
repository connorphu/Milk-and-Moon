import { HttpClient } from '@angular/common/http'
import { inject, Service } from '@angular/core'
import { toSignal } from '@angular/core/rxjs-interop'
import { environment } from '../../../environments/environment'
import { BabyResponse } from '../models/baby'

@Service()
export class BabyService {
  private http = inject(HttpClient)
  private url = `${environment.apiUrl}/babies`
  readonly babies = toSignal(this.http.get<BabyResponse[]>(this.url), { initialValue: [] })
}
