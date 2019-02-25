import { Component, OnInit } from '@angular/core';
import {Exhibitor} from "../models/exhibitor.model";
import {Category} from "../models/category.model";
import {CategoriesDataService} from "../categories/categories-data.service";
import {ExhibitorsDataService} from "../exhibitors/exhibitors-data.service";
import {QuestionSharedService} from "../questions/question-shared.service";
import {QuestionDataService} from "../questions/question-data.service";
import {Router} from "@angular/router";
import {BsModalService} from "ngx-bootstrap";

@Component({
  selector: 'app-upload-csv',
  templateUrl: './upload-csv.component.html',
  styleUrls: ['./upload-csv.component.css']
})
export class UploadCsvComponent implements OnInit {

  constructor(private _categoryDataService: CategoriesDataService,private _exhibitorDataService: ExhibitorsDataService, private _questionSharedService: QuestionSharedService,
              private _questionDataService: QuestionDataService, private router: Router,
              private modalService: BsModalService) { }


  ngOnInit() {
  }

  onSelectFile(event) { // called each time file input changes
    var files = event.target.files; // FileList object
    var file = files[0];
    var reader = new FileReader();
    reader.readAsText(file);
    reader.onload = (event: any) => { // called once readAsDataURL is completed
      this._csv = event.target.result;
      this.fileSelected = true;
    }
  }

  set csv(file) {
    this._csv = file;
  }

  get csv() {
    return this._csv;
  }

  private _csv;
  fileSelected = false;


  private exhibitor: Exhibitor;
  private exhibitors : Exhibitor[] = [];
  private categoriess : Category[] = [];
  private categorie: Category;
  private extractData(data) { // Input csv data to the function
    this._questionDataService.removeQuestions().subscribe(questions => {
    });
    this._exhibitorDataService.removeExhibitors().subscribe(exhibitors => {
    });
    /*this._categoryDataService.removeCategories().subscribe(categories => {
    });*/

    let csvData = data;
    let allTextLines = csvData.split(/\r\n|\n/);
    let headers = allTextLines[0].split(';');
    let lines = [];
    let exhibitorToAdd: Exhibitor;
    let categoryToAdd: Category;
    let teller = 1;


    /*for ( let i = 1; i < allTextLines.length; i++) {
      let data = allTextLines[i].split(';');
      if (data.length == headers.length) {
        let tarr = [];
        for ( let j = 0; j < headers.length; j++) {
          tarr.push(data[j]);
        }
        lines.push(tarr);

      }
      categoryToAdd = this.categoriess.find(x=>x.name == data[2]);
      if(categoryToAdd == null){
        this.categorie = new Category();
        this.categorie.name = data[2];
        this.categorie.description = "";
        this.categoriess.push(this.categorie);
      }
    }

    for(let cat of this.categoriess){
      this._categoryDataService.voegCategorieToe(cat).subscribe(cate => {
      });
    }*/



    for ( let i = 1; i < allTextLines.length; i++) {
      let data = allTextLines[i].split(';');
      if (data.length == headers.length) {
        let tarr = [];
        for ( let j = 0; j < headers.length; j++) {
          tarr.push(data[j]);
        }
        lines.push(tarr);

      }
      exhibitorToAdd = this.exhibitors.find(x=>x.name == data[0]);

      if (this.exhibitors.length != 0 && exhibitorToAdd != undefined) {
        this._categoryDataService.categorieByName(data[2]).subscribe(categoryByName => {
          exhibitorToAdd.categories.push(categoryByName);
        });
      } else {
        this._categoryDataService.categorieByName(data[2]).subscribe(categoryByName => {
          exhibitorToAdd = this.exhibitors.find(x=>x.name == data[0]);
          if(exhibitorToAdd != null) {
            categoryToAdd = exhibitorToAdd.categories.find(x=>x.name == data[2]);
            if(categoryToAdd == null) {
              exhibitorToAdd.categories.push(categoryByName);
            }
            teller++;
          } else {
            this.exhibitor = new Exhibitor();
            this.exhibitor.x = 0;
            this.exhibitor.y = 0;
            this.exhibitor.name = data[0];
            if(data[1] == ""){
              this.exhibitor.exhibitorNumber = "exposantnummer is niet ingevuld.";
            } else{
              this.exhibitor.exhibitorNumber = data[1];
            }
            this.exhibitor.categories.push(categoryByName);
            this.exhibitors.push(this.exhibitor);
            if(allTextLines.length-4< teller){
              console.log("toe te voegen categorieen");
              console.log(this.categoriess);
              for(let exhib of this.exhibitors){
                this._exhibitorDataService.voegExhibitorToe(exhib).subscribe(exposant => {
                });
              }
              for ( let i = 1; i < allTextLines.length; i++) {
                let data = allTextLines[i].split(';');
                if (data.length == headers.length) {
                  let tarr = [];
                  for ( let j = 0; j < headers.length; j++) {
                    tarr.push(data[j]);
                  }
                  lines.push(tarr);

                }

                this._categoryDataService.categorieByName(data[2]).subscribe(categoryByName => {
                  this._exhibitorDataService.exhibitorByName(data[0]).subscribe(exhibitorByName => {
                    if(data[3] == "") {
                      let question = {
                        "questionText": "geen vraag",
                        "answerText": "geen antwoord",
                        "exhibitorId": exhibitorByName.id,
                        "categoryId": categoryByName.id
                      };
                      this._questionDataService.addNewQuestion(question).subscribe(question => {
                      });
                    } else if(data[4] == "") {
                      let question = {
                        "questionText": data[3],
                        "answerText": "geen antwoord",
                        "exhibitorId": exhibitorByName.id,
                        "categoryId": categoryByName.id
                      };
                      this._questionDataService.addNewQuestion(question).subscribe(question => {
                      });
                    } else {
                      let question = {
                        "questionText": data[3],
                        "answerText": data[4],
                        "exhibitorId": exhibitorByName.id,
                        "categoryId": categoryByName.id
                      };
                      this._questionDataService.addNewQuestion(question).subscribe(question => {
                      });
                    }
                  });
                });
              }
            }
            teller++;
          }
        });
      }
    }
    this.router.navigateByUrl('/questions', {skipLocationChange: true}).then(()=>
      this.router.navigate(["/"]));
  }

  /*public replaceAll(input: string, find: string, replace: string): string {
    return input.replace(new RegExp(find, 'g'), replace);
  }*/

  public uploadCSV() {
    this.extractData(this._csv);
  }
}
