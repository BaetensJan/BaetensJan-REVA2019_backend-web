import {Component, OnInit, TemplateRef} from '@angular/core';
import {QuestionDataService} from "./question-data.service";
import {Question} from "../models/question.model";
import {BsModalRef, BsModalService} from "ngx-bootstrap";
import {Router} from "@angular/router";
import {QuestionSharedService} from "./question-shared.service";
import {Category} from "../models/category.model";
import {Exhibitor} from "../models/exhibitor.model";
import {CategoriesDataService} from "../categories/categories-data.service";
import {ExhibitorsDataService} from "../exhibitors/exhibitors-data.service";
import {forEach} from "@angular/router/src/utils/collection";

@Component({
  selector: 'app-questions',
  templateUrl: './questions.component.html',
  styleUrls: ['./questions.component.css']
})
export class QuestionsComponent implements OnInit {

  /**
   * Constructor
   *
   * @param router: Router
   * @param modalService: BsModalService
   * @param _categoryDataService: CategoriesDataService
   * @param _questionSharedService: QuestionShareService
   * @param _questionDataService: QuestionDataService
   */
  constructor(private _categoryDataService: CategoriesDataService,private _exhibitorDataService: ExhibitorsDataService, private _questionSharedService: QuestionSharedService,
              private _questionDataService: QuestionDataService, private router: Router,
              private modalService: BsModalService) {
  }

  /**
   * All the questions from the database.
   */
  private _allQuestions: Question[];
  /**
   * Questions that meet the current selected category
   */
  private _currentCategoryQuestions: Question[];
  private _allCategories: Category[];
  /**
   * List of questions that will be displayed and meet the current filter and current selected category.
   * This is a segment of the _currentCategoryQuestions.
   */
  private _filteredQuestions: Question[];
  private selectedCategory: Category;
  private _filterValue: string = "";

  /**
   * @ignore
   */
  refModal: BsModalRef;

  /**
   * The question that the user wants to delete.
   */
  clickedItem: Question;

  clickedItemNaam : string;

  ngOnInit() {
    this._questionDataService.questions.subscribe(value => {
      this._allQuestions = value;
      this._categoryDataService.categories.subscribe(value1 => {
        this._allCategories = value1;

        this.selectedCategory = null; // all categories will be displayed.
        this._currentCategoryQuestions = this._allQuestions;
        this._filteredQuestions = this._currentCategoryQuestions;
      });
    });
  }

  public filter(token: string) {
    this._filteredQuestions = this._currentCategoryQuestions.filter((question: Question) => {
        return question.exhibitor.name.toLowerCase().startsWith(token.toLowerCase()) ||
          question.category.name.toLowerCase().startsWith(token.toLowerCase());
      });
  }

  get selectedCategoryName(): string {
    return this.selectedCategory == null ? "alle categorieën" : this.selectedCategory.name;
  }

  selectedCategoryChanged(cat: Category) {
    if (cat == null) {
      this.selectedCategory = null;
      this._currentCategoryQuestions = this._allQuestions;
      this.filter(this._filterValue);
      return;
    }
    this.selectedCategory = cat;
    this._currentCategoryQuestions = this._allQuestions.filter(q => q.category.id == this.selectedCategory.id);
    this.filter(this._filterValue);
  }

  get selectedExhibitors(): Exhibitor[] {
    // We need to check via the list of all the questions,
    // in order to make sure that the exhibitor has a question (an exhibitor could be created without a question) and
    // thus could potentially be an empty table (without questions) in questions.component.html.
    let exhibs: Exhibitor[] = [];
    this._filteredQuestions.forEach(q => {
      if (exhibs.findIndex(ex => ex.id == q.exhibitor.id) < 0)
        exhibs.push(q.exhibitor);
    });
    return exhibs.sort((a : Exhibitor, b: Exhibitor) => a.name > b.name ? 1 : -1);
  }

  /**
   * Returns the list of questions
   */
  get questions(): Question[] {
    return this._filteredQuestions;
  }

  get categories(): Category[] {
    return this._allCategories;
  }

  /**
   * Click event to create and add a new question.
   *
   */
  addQuestion() {
    this._questionSharedService.question = null;
    this.router.navigate(["/vraag"]);
  }

  /**
   * Click event for editing a question. Geeft de categorie door aan share service om zo de data te kunnen verkrijgen in het categorie component.
   *
   * @param row: Categorie
   */
  editQuestion(row: Question) {
    this._questionSharedService.question = row;
    this._questionSharedService.edit = true;
    this.router.navigate(["/vraag"]);
  }

  /**
   * Click event to ask user if he is sure the question may be deleted.
   *
   * @param row: Categorie
   */
  openModal(template: TemplateRef<any>, question: Question) {
    this.clickedItem = question;
    this.clickedItemNaam = question.questionText;
    this.refModal = this.modalService.show(template, {class: 'modal-sm'});
  }

  /**
   * Toggle voor modal te tonen.
   *
   */
  hideModal() {
    this.refModal.hide();
  }

  /**
   * Bevestiging van het verwijderen van een categorie. Dit wordt gebruikt bij het modal om te bevestigen.
   *
   */
  deleteConfirmed() {
    this.refModal.hide();
    this._questionDataService.removeQuestion(this.clickedItem).subscribe(question => {
      let index = this._allQuestions.indexOf(this.clickedItem);
      this._allQuestions.splice(index, 1);
      index = this._filteredQuestions.indexOf(this.clickedItem);
      this._filteredQuestions.splice(index, 1);
      index = this._currentCategoryQuestions.indexOf(this.clickedItem);
      this._currentCategoryQuestions.splice(index, 1);
      this.clickedItem = null;
    });
    window.location.reload();
  }


  get filterValue(): string {
    return this._filterValue;
  }

  set filterValue(value: string) {
    this._filterValue = value;
  }
}
