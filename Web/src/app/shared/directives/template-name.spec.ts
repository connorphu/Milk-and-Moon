import { Component, TemplateRef, viewChild } from '@angular/core'
import { TemplateName } from './template-name'
import { TestBed } from '@angular/core/testing'

@Component({
  imports: [TemplateName],
  template: `<ng-template template-name="test-this">content</ng-template>`
})
class TestHost {
  readonly directive = viewChild.required(TemplateName)
}

describe('TemplateName', () => {
  let host: TestHost

  beforeEach(async () => {
    const fixture = TestBed.createComponent(TestHost)
    host = fixture.componentInstance
    await fixture.whenStable()
  })

  it('should read template-name attribute as TemplateName', () => {
    expect(host.directive().templateName()).toBe('test-this')
  })

  it('should capture the TemplateRef of its ng-template', () => {
    expect(host.directive().templateRef).toBeInstanceOf(TemplateRef)
  })
})
