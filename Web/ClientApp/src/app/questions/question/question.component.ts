import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {Question} from "../../models/question.model";
import {QuestionSharedService} from "../question-shared.service";
import {QuestionDataService} from "../question-data.service";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {Exhibitor} from "../../models/exhibitor.model";
import {ExhibitorsDataService} from "../../exhibitors/exhibitors-data.service";
import {CategoriesDataService} from "../../categories/categories-data.service";
import {Category} from "../../models/category.model";

@Component({
  selector: 'app-question',
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css']
})
export class QuestionComponent implements OnInit {
  /**
   * @ignore
   */
  public question: FormGroup;
  /**
   * @ignore
   */
  public _categories: Category[];
  /**
   * @ignore
   */
  public _exhibitors: Exhibitor[];
  /**
   * @ignore
   */
  public edit: boolean;

  /**
   * Constructor
   *
   * @param router
   * @param _questionSharedService
   * @param _questionDataService
   * @param _categoryDataService
   * @param _exhibitorDataService
   * @param fb
   */
  constructor(private fb: FormBuilder, private router: Router,
              private _questionSharedService: QuestionSharedService,
              private _questionDataService: QuestionDataService,
              private _categoryDataService: CategoriesDataService,
              private _exhibitorDataService: ExhibitorsDataService) {
  }

  ngOnInit() {
    this._categoryDataService.categories.subscribe(categories => {
      this._categories = categories.sort((a: Category, b: Category) => a.name > b.name ? 1 : -1);
      this._exhibitorDataService.exhibitors.subscribe(exhibitors => {
        this._exhibitors = exhibitors.sort((a: Exhibitor, b: Exhibitor) => a.name > b.name ? 1 : -1);
        this.edit = this._questionSharedService.edit;

        if (this.edit) {
          this.setCorrectExhibitorAndCategory(this._questionSharedService.question);
        }
        this.prepareForm(this.edit ? this._questionSharedService.question :
          new Question("", "", this._categories[0], this._exhibitors[0]));
      })
    })
  }

  /**
   * This method is used to work with the correct exhibitor and category objects in _exhibitors and _categories,
   * as the exhibitor and category object of the question to edit (from the _questionSharedService) is slightly
   * different, and thus not recognised in the html, resulting in not selecting it in the select element.
   *
   * @param question
   */
  private setCorrectExhibitorAndCategory(question: Question) {
    question.category = this._categories.find(c => c.id == question.category.id);
    question.exhibitor = this._exhibitors.find(c => c.id == question.exhibitor.id);
  }

  private prepareForm(question: Question) {
    this.question = this.fb.group({
      questionText: [question.questionText, Validators.compose([Validators.required, Validators.minLength(1)])],
      answerText: [question.answer, Validators.compose([Validators.required, Validators.minLength(1)])],
      exhibitor: [question.exhibitor, Validators.compose([Validators.required])],
      category: [question.category, Validators.compose([Validators.required])],
    });
  }

  /**
   * Click event to save or create Category
   */
  onSubmit() {
    let question = {
      "questionText": this.question.value.questionText,
      "answerText": this.question.value.answerText,
      "exhibitorId": this.question.value.exhibitor.id,
      "categoryId": this.question.value.category.id
    };

    if (!this.edit) {
      this._questionDataService.addNewQuestion(question).subscribe(question => {
        this.goToQuestions();
      });
    } else {
      let question2 = {
        "questionId": this._questionSharedService.question.id,
        "questionText": this.question.value.questionText,
        "answerText": this.question.value.answerText,
        "exhibitorId": this.question.value.exhibitor.id,
        "categoryId": this.question.value.category.id
      };
      this._questionDataService.editQuestion(question2).subscribe(question => {
        this.goToQuestions();
      });
    }
  }

  goToQuestions() {
    this.router.navigate(["/vragen"]);
  }

  get exhibitors() {
    return this._exhibitors;
  }

  get categories() {
    return this._categories;
  }
}


