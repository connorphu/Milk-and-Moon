import { BabyLog } from './baby-log'
import { environment } from '../../../environments/environment'
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing'
import { TestBed } from '@angular/core/testing'
import { provideHttpClient } from '@angular/common/http'
import { BabyLogModel } from '../models/baby-log'

describe('BabyLog', () => {
  let service: BabyLog
  let httpMock: HttpTestingController
  const url = `${environment.jsonServer}/logs`

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    })
    service = TestBed.inject(BabyLog)
    httpMock = TestBed.inject(HttpTestingController)
  })

  afterEach(() => {
    httpMock.verify()
  })

  it('should be created', () => {
    expect(service).toBeTruthy()
  })

  describe('loadData', () => {
    it('should GET all logs', () => {
      const fakeLogs = [{ trackerType: 'feed' }] as unknown as BabyLogModel[]
      let result: BabyLogModel[] | undefined

      service.loadData().subscribe((logs) => (result = logs))

      const req = httpMock.expectOne(url)
      expect(req.request.method).toBe('GET')

      req.flush(fakeLogs)

      expect(result).toEqual(fakeLogs)
    })
  })

  describe('editData', () => {
    const fakeLog = { id: '24', trackerType: 'feed' } as unknown as BabyLogModel
    it('should PUT log to its id and return true', () => {
      let result: boolean | undefined

      service.editData(fakeLog).subscribe((res) => (result = res))

      const req = httpMock.expectOne(`${url}/${fakeLog.id}`)
      expect(req.request.method).toBe('PUT')
      expect(req.request.body).toEqual(fakeLog)

      req.flush(fakeLog)

      expect(result).toBe(true)
    })

    it('should return false when request fails', () => {
      let result: boolean | undefined

      service.editData(fakeLog).subscribe((res) => (result = res))

      httpMock
        .expectOne(`${url}/${fakeLog.id}`)
        .flush('Server error', { status: 500, statusText: 'Server error' })

      expect(result).toBe(false)
    })
  })

  describe('track', () => {
    it('should save log', () => {})
  })

  describe('deleteData', () => {})
})
