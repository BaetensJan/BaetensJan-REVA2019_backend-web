import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/Observable';
import {map} from 'rxjs/operators';
import {Question} from "../models/question.model";

@Injectable({
  providedIn: 'root'
})
export class QuestionDataService {
  /**
   * Base url of the to connected connection
   */
  private readonly _appUrl = '/API/Question';

  /**
   * Constructor
   *
   * @param http
   */
  constructor(private http: HttpClient) {}

  /**
   * Getter for Groups
   */
  get questions(): Observable<Question[]> {
    return this.http
      .get(`${this._appUrl}/Questions/`)
      .pipe(map((list: any[]): Question[] => list.map(Question.fromJSON)));
  }

  /**
   * Makes call to the backend and adds new Question
   *
   * @param question
   */
  addNewQuestion(question: any): Observable<Question> {
    return this.http
      .post(`${this._appUrl}/createQuestion/`, question)
      .pipe(map(Question.fromJSON)); //Todo bij falen van edit => errortext weergeven idpv mappen naar question.
  }

  /**
   * Makes call to the backend and edit a question.
   *
   * @param question
   */
  editQuestion(question: any): Observable<Question> {
    return this.http
      .put(`${this._appUrl}/EditQuestion/${question.questionId}`, question)
      .pipe(map(Question.fromJSON)); //Todo bij falen van edit => errortext weergeven idpv mappen naar question.
  }

  /**
   * Makes call to the backend and removes a question
   * @param rec
   */
  removeQuestion(rec: Question): Observable<Question> {
    return this.http
      .delete(`${this._appUrl}/deleteQuestion/${rec.id}`)
      .pipe(map(Question.fromJSON));
  }

  /**
   * Makes call to the backend and removes all the questions
   * @param rec
   */
  removeQuestions(): Observable<Question[]> {
    return this.http
      .delete(`${this._appUrl}/deleteQuestions`)
      .pipe(map((list: any[]): Question[] => list.map(Question.fromJSON)));
  }

  /**
   * Makes call to the backend and returns a question
   *
   * @param id
   */
  getQuestion(id: string): Observable<Question> {
    return this.http
      .get(`${this._appUrl}/getQuestion/${id}`)
      .pipe(map(Question.fromJSON));
  }
}
