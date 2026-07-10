import { Directive, input, inject, TemplateRef } from '@angular/core';

@Directive({
  selector: 'ng-template[template-name]',
  standalone: true
})
export class TemplateName {
  // Grab the actual TemplateRef via dependency injection
  public templateRef = inject(TemplateRef);

  // Use a stable signal input to capture the string key
  public templateName = input.required<string>({ alias: 'template-name' });
}
