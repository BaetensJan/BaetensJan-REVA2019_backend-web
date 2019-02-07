import * as jsPDF from "jspdf";
import {Group} from "../models/group.model";
import {Component} from "@angular/core";
import * as JSZip from "jszip";
import {saveAs} from 'file-saver';
import {Router} from "@angular/router";
import {School} from "../models/school.model";
import {GroupsDataService} from "../groups/groups-data.service";
import * as html2canvas from 'html2canvas';
import * as pdfcrowd from 'pdfcrowd';
import {Observable, Observer} from "rxjs";
import {Question} from "../models/question.model";
import {Assignment} from "../models/assignment.model";
import {$} from "protractor";
import {AssignmentDetailComponent} from "../assignment-detail/assignment-detail.component";

function parseJwt(token) {
  if (!token) {
    return null;
  }
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    return JSON.parse(window.atob(base64));
  } catch (err) {
    return null;
  }
}

@Component({
  selector: 'app-answer',
  templateUrl: './assignments.component.html',
  styleUrls: ['./assignments.component.css']
})
export class AssignmentsComponent {

  school: School;
  groups: Group[];
  maxNumberOfGroupsPerPage = 5; // amount of groups that will be showed on the current page, to keep the page neat.
  contentArray: Group[]; // array containing the groups that fits the filter.
  returnedArray: Group[]; // array containing the groups (maxNumberOfGroupsPerPage) that are showed on the current page.
  private _filteredGroups: Group[];
  filterValue = "";

  /**
   * Constructor
   * @param router
   * @param _groupsDataService
   */
  constructor(
    private router: Router, private _groupsDataService: GroupsDataService
    /* private _assignmentDataService: AssignmentDataService*/) {
  }

  ngOnInit(): void {
    /**
     * Gets the school object of the teacher that is logged in.
     * This school object consists of the finished assignments from the backend for each group and adds this to the table
     */
    this.filterValue = "";
    let currentUser = parseJwt(localStorage.getItem("currentUser"));
    let schoolId = currentUser.school;
    let isAdmin = currentUser.isAdmin;

    if (isAdmin == "True") {
      this._groupsDataService.groups.subscribe(value => {
        this.groups = value;
        this.maxNumberOfGroupsPerPage = this.groups.length;
        this.contentArray = this.groups;
        this._filteredGroups = this.groups;
        this.initiateReturnedArray();
      });
    } else {
      this._groupsDataService.groupsBySchoolId(schoolId).subscribe(value => {
        this.groups = value;
        this.maxNumberOfGroupsPerPage = this.groups.length;
        this.contentArray = this.groups;
        this.initiateReturnedArray();
        this._filteredGroups = this.groups;
      });
    }
  }

  /**
   * initiates the returnedArray with the groups that should be presented on the current page.
   */
  initiateReturnedArray() {
    this.returnedArray = this.contentArray.slice(0, this.maxNumberOfGroupsPerPage);
  }


  /**
   * Generates pdf file for group and downloads pdf
   *
   * @param row
   */
  pdfDownload(row: Group) {
    const doc = new jsPDF();
    doc.text("Groep: " + row.name, 10, 10);
    doc.text("Aantal bezochte standen: " + row.assignments.length, 10, 20);
    let i = 0;
    row.assignments.forEach(f => {
        if (i == 0) {
          if (i > 0) {
            doc.addPage();
          }
          console.log(f.question.category);
          doc.text("Stand: " + (i + 1) + " :" + f.question.exhibitor.name, 10, 40);

          if (!f.extra) {
            doc.text("Categories: " + f.question.category.name, 10, 50);
            //doc.text("categories: " + f.exhibitor.categories.map(a => a.name).join(", "), 10, 50);
            doc.text("Vraag: " + f.question.questionText, 10, 60);
          }

          doc.text("Antwoord: " + f.answer, 10, 70);
          doc.text("Notities: " + f.notes, 10, 80);
          if (f.photo != null) {
            this.addImage(f.photo, doc); //Todo: dit werkt na de 2x keer klikken, prolly iets met caching te maken.
            setTimeout(() => {
              console.log('Test');
              // this.addImage(f.photo, doc);
            }, 100000);
          }
          i++;
        }
        else {
          if (i > 0) {
            doc.addPage();
          }
          if (!f.extra) {
            doc.text("Stand " + (i + 1) + " :" + f.question.exhibitor.name, 10, 10);
            doc.text("Categories: " + f.question.category.name, 10, 20);
            //doc.text("categories: " + f.exhibitor.categories.map(a => a.name).join(", "), 10, 20);
            doc.text("Vraag: " + f.question.questionText, 10, 50);
          }
          doc.text("Antwoord: " + f.answer, 10, 60);
          doc.text("Notities: " + f.notes, 10, 70);
          if (f.photo != null) {
            this.addImage(f.photo, doc); //Todo: dit werkt na de 2x keer klikken, prolly iets met caching te maken.
            setTimeout(() => {
              console.log('Test');
              // this.addImage(f.photo, doc);
            }, 100000);
          }
          i++;
        }
      }
    );
    doc.save(row.name + "_antwoorden.pdf");
  }

  /**
   * Generates pdf files for all groups and download all the pdf's
   */
  downloadAllPDF() {
    let teller = 0;
    var zip = new JSZip();
    for (let i = 0; i < this.groups.length; i++) {
      let row = this.groups[teller];
      const doc = new jsPDF();
      doc.text("Groep: " + row.name, 10, 10);
      doc.text("Aantal bezochte standen: " + row.assignments.length, 10, 20);
      let i = 0;
      // item.finishedAssignments.forEach(f => {
      row.assignments.forEach(f => {
        if (i == 0) {
          if (i > 0) {
            doc.addPage();
          }
          if (!f.extra) {
            doc.text("Stand " + (i + 1) + " :" + f.question.exhibitor.name, 10, 40);
            doc.text("categorie: " + f.question.category.name, 10, 50);
            doc.text("Vraag: " + f.question.questionText, 10, 60);
          }
          doc.text("Antwoord: " + f.answer, 10, 70);
          doc.text("Notities: " + f.notes, 10, 80);
          if (f.photo != null) {
            var data = document.getElementById("imageid");
            html2canvas(data).then(canvas => {
              var imgconverted = canvas.toDataURL("data:image/png;base64");
              console.log(imgconverted, 'base64');

              setTimeout(() => {    //<<<---    using ()=> syntax
              }, 300000);
              doc.addImage(imgconverted, "PNG", 10, 90, 100, 100);
            });
          }
          i++;
        } else {
          if (i > 0) {
            doc.addPage();
          }
          if (!f.extra) {
            doc.text("Stand " + (i + 1) + " :" + f.question.exhibitor.name, 10, 10);
            doc.text("categorie: " + f.question.category.name, 10, 20);
            doc.text("Vraag: " + f.question.questionText, 10, 60);
          }
          doc.text("Antwoord: " + f.answer, 10, 70);
          doc.text("Notities: " + f.notes, 10, 50);
          if (f.photo != null) {
            var data = document.getElementById("imageid");
            html2canvas(data).then(canvas => {
              var imgconverted = canvas.toDataURL("data:image/png;base64");
              console.log(imgconverted, 'base64');

              setTimeout(() => {    //<<<---    using ()=> syntax
              }, 300000);
              doc.addImage(imgconverted, "PNG", 10, 90, 100, 100);
            });
          }
          i++;
        }
      });
      zip.file(row.name + "_antwoorden.pdf", doc.output());

      teller++;
    }
    zip.generateAsync({type: "blob"}).then(function (content) {
      // see FileSaver.js
      saveAs(content, "alle_antwoorden.zip");
    });
  }

  detailAssignment(group: Group) {
    this.router.navigate(["/assignmentdetail"], {queryParams: {groupId: group.id}});
  }

  public filter(token: string) {
    this._filteredGroups = this.groups.filter((group: Group) => {
      return group.name.toLowerCase().startsWith(token.toLowerCase());

      //return question.categoryExhibitor.exhibitor.name.toLowerCase().startsWith(token.toLowerCase()) ||
      //  question.categoryExhibitor.category.name.toLowerCase().startsWith(token.toLowerCase());
    });
  }

  private getBase64ImageFromURL(url: string) {
    return Observable.create((observer: Observer<string>) => {
      // create an image object
      let img = new Image();
      img.crossOrigin = 'Anonymous';
      img.src = url;
      if (!img.complete) {
        // This will call another method that will create image from url
        img.onload = () => {
          observer.next(this.getBase64Image(img));
          observer.complete();
        };
        img.onerror = (err) => {
          observer.error(err);
        };
      } else {
        observer.next(this.getBase64Image(img));
        observer.complete();
      }
    });
  }

  private getBase64Image(img: HTMLImageElement) {
    // We create a HTML canvas object that will create a 2d image
    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext("2d");
    // This will draw image
    ctx.drawImage(img, 0, 0);
    // Convert the drawn image to Data URL
    var dataURL = canvas.toDataURL("image/png");
    return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
  }

  private addImage(photoURL, doc) {
    let data = `/images/${photoURL}`;
    this.getBase64ImageFromURL(data).subscribe(imageCanvas => {
      let formData = new FormData();
      formData.append('file', imageCanvas);
      let imgconverted = formData.get("file");

      doc.addImage(imgconverted, "PNG", 10, 90, 100, 100);
    });
  }
}
