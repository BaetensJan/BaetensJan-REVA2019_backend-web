import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators} from "@angular/forms";
import {Router} from "@angular/router";
import {HttpErrorResponse} from "@angular/common/http";
import {QuestionService} from "./question.service";

@Component({
  selector: 'app-ask-question',
  templateUrl: './ask-question.component.html',
  styleUrls: ['./ask-question.component.css']
})
export class AskQuestionComponent implements OnInit {

  /**
   * @ignore
   */
  public question: FormGroup;
  /**
   * @ignore
   */
  public errorMsg: string;

  /**
   * Constructor
   *
   * @param router
   * @param fb
   * @param _questionService
   */
  constructor(
    private router: Router,
    private fb: FormBuilder,
    private _questionService: QuestionService
  ) {
  }

  /**
   * Setup for username registration form
   */
  ngOnInit() {
    this.question = this.fb.group({
      email: [
        '',
        Validators.compose([
          Validators.required,
          this.emailPatternValidator()])
      ],
      subject: [
        '',
        [Validators.required,
          Validators.minLength(1)]
      ],
      message: [
        '',
        [Validators.required,
          Validators.minLength(1)]
      ],
    });
  }

  emailPatternValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } => {
      const email = control.value;
      let regexp = new RegExp(
        /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
      //"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");
      let correctInput = regexp.test(email);
      return correctInput ? null : {wrongInput: true};
    };
  }

  /**
   * Click event to submit question to the backend
   */
  onSubmit() {
    this._questionService
      .sendQuestion(this.question.value.email, this.question.value.subject, this.question.value.message)
      .subscribe(
        val => {
          if (val) {
            this.router.navigate(['/']);
          }
        },
        (error: HttpErrorResponse) => {
          this.errorMsg = `Er is iets fout gegaan bij de verwerking van uw aanvraag. Gelieve het later opnieuw te proberen.`;
        }
      );
  }
}
