import * as jsPDF from "jspdf";
import {ChangeDetectorRef, Component, ElementRef, ViewChild} from "@angular/core";
import * as JSZip from "jszip";
import {saveAs} from 'file-saver';
import {Router} from "@angular/router";
import {Observable, Observer} from "rxjs";
import {Group} from "../../models/group.model";
import {GroupsDataService} from "../../groups/groups-data.service";
import {PageChangedEvent} from 'ngx-bootstrap/pagination';
import {School} from "../../models/school.model";
import {SchoolDataService} from "../../schools/school-data.service";
import {Assignment} from "../../models/assignment.model";

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

  @ViewChild('myAssignmentDiv') myAssignmentDiv: ElementRef;


  currentPage = 1; // the current page in the pagination (e.g. page 1, 2 ...)
  getIndex(index: number): number {
    return (this.currentPage - 1) * this.maxNumberOfGroupsPerPage + index + 1;
  }

  private _isAdmin: string;
  get isAdmin(): boolean {
    return this._isAdmin == "True";
  }

  private _groups: Group[] = [];

  get totalNumberOfAssignments(): number {
    return this._groups.length;
  }

  maxNumberOfGroupsPerPage = 5; // amount of groups that will be showed on the current page, to keep the page neat.

  private _returnedArray: Group[]; // array containing the groups (maxNumberOfGroupsPerPage) that are shown on the current page.

  /**
   * Getter for groups
   */
  get returnedArray(): Group[] {
    return this._returnedArray;
  }

  private _filteredGroups: Group[]; // array containing all groups that meet the filter

  /**
   * Getter for groups
   */
  get filteredGroups(): Group[] {
    return this._filteredGroups;
  }

  filterValue = "";

  /**
   * Constructor
   * @param router
   * @param _groupsDataService
   * @param _schoolDataService
   * @param _cdref
   */
  constructor(
    private router: Router,
    private _groupsDataService: GroupsDataService,
    private _schoolDataService: SchoolDataService,
    private _cdref: ChangeDetectorRef) {
  }

  ngOnInit(): void {
    /**
     * Gets the school object of the teacher that is logged in.
     * This school object consists of the finished assignments from the backend for each group and adds this to the table
     */
    this.filterValue = "";
    let currentUser = parseJwt(localStorage.getItem("currentUser"));
    let schoolId = currentUser.school;
    this._isAdmin = currentUser.isAdmin;

    if (this.isAdmin) {
      this._schoolDataService.schools().subscribe((schools: School[]) => {
        for (let i = 0; i < schools.length; i++) {
          let school = schools[i];
          for (let j = 0; j < school.groups.length; j++) {
            let group = school.groups[j];

            if (group.assignments.length > 0) {
              group.schoolLoginName = school.name;

              this._groups.push(group);
            }
          }
        }

        this.initiateArrays();
      });
    } else {
      this._groupsDataService.groupsBySchoolId(schoolId).subscribe((groups: Group[]) => {
        for (let i = 0; i < groups.length; i++) {
          let group = groups[i];

          if (group.assignments.length > 0) {
            this._groups.push(group);
          }
        }
        this.initiateArrays();
      });
    }
  }

  /**
   * When a user clicks on the next page pagination button,
   * it will show the next groups.
   * @param event
   */
  pageChanged(event: PageChangedEvent): void {
    this.filterValue = "";
    const startItem = (event.page - 1) * event.itemsPerPage;
    const endItem = event.page * event.itemsPerPage;
    this._returnedArray = this._filteredGroups.slice(startItem, endItem);
  }

  /**
   * Sorts the groups alphabetically
   */
  initiateArrays() {
    this._groups.sort();
    this._filteredGroups = this._groups;
    this._returnedArray = this._filteredGroups.slice(0, this.maxNumberOfGroupsPerPage);
  }


  /**
   * Generates pdf file for group and downloads pdf
   *
   * @param row
   */
  async pdfDownload(row: Group) {
    let doc = new jsPDF();
    let page = 1;

    for (let i = 0; i < row.assignments.length; i++) {
      doc.setPage(page);
      await this.getAssignment(doc, row.assignments[i], i + 1);
      page++;
      doc.addPage("a4", "p");
    }
    doc.deletePage(page);
    await doc.save(`${row.name}.pdf`)
  }

  async getAssignment(doc: jsPDF, assignment: Assignment, index: number) {
    console.log(assignment);
    doc.setFontSize(16);
    doc.text(20, 20, this.splitText(`Opdracht ${index}`));
    doc.setFontSize(12);
    let array = [];
    array.push('');
    array.push('');
    array.push(...this.splitText(`Stand: ${assignment.question.exhibitor.name}`));
    array.push('');
    array.push(...this.splitText(`Categorie: ${assignment.question.category.name}`));
    array.push('');
    array.push(...this.splitText(`Vraag: ${assignment.question.questionText}`));
    array.push('');
    array.push(...this.splitText(`Antwoord van groep: ${assignment.answer}`));
    array.push('');
    array.push(...this.splitText(`Model antwoord: ${assignment.question.answer}`));
    array.push('');
    array.push(...this.splitText(`Notities: ${assignment.notes}`));
    array.push('');
    array.push(...this.splitText(`Foto:`));
    array.push('');
    doc.text(20, 20, array);
    if (assignment.photo) {
      try {
        let width = 0;
        let height = 0;
        let dataURL: any = await new Promise((resolve, reject) => {
          let img = new Image();
          img.setAttribute('crossOrigin', 'anonymous');
          img.onload = function () {
            let canvas = document.createElement('canvas');
            canvas.width = img.naturalWidth; // or 'width' if you want a special/scaled size
            canvas.height = img.naturalHeight; // or 'height' if you want a special/scaled size

            canvas.getContext('2d').drawImage(img, 0, 0);
            resolve(canvas.toDataURL('image/png'));

            width = canvas.width;
            height = canvas.height;
          };
          img.onerror = reject;
          img.src = `${'/images/' + assignment.photo}`;

        });
        if (dataURL) {
          const scale = 100;
          doc.addImage(dataURL, 'JPEG', 20, 20 + (array.length * 5), width / width * scale, height / width * scale);
        }
      } catch (e) {
      }
    }
  }

  /**
   * Generates pdf files for all groups and download all the pdf's
   */
  async downloadAllPDF() {
    let doc, page, row;
    let zip = new JSZip();

    for (let j = 0; j < this._groups.length; j++) {
      doc = new jsPDF();
      page = 1;
      row = this._groups[j];

      for (let i = 0; i < row.assignments.length; i++) {
        doc.setPage(page);
        await this.getAssignment(doc, row.assignments[i], i + 1);
        page++;
        doc.addPage("a4", "p");
      }
      doc.deletePage(page);
      await zip.file(row.name + "_antwoorden.pdf", await doc.output('blob'));
    }
    let content = await zip.generateAsync({type: "blob"});
    saveAs(content, "alle_antwoorden.zip");
  }

  detailAssignment(group: Group) {
    this.router.navigate(["/assignmentdetail"], {queryParams: {groupId: group.id}});
  }

  /** FILTER **/
  public filter(token: string) {
    token = token.toLowerCase();
    if (!token) {
      this._filteredGroups = this._groups;
    } else {
      this._filteredGroups = this._groups.filter((group: Group) => {
        return group.name.toLowerCase().includes(token);
      });
    }
    this._returnedArray = this._filteredGroups.slice(0, this.maxNumberOfGroupsPerPage);
    console.log("current page: ", this.currentPage);

    // we need to detectChanges, otherwise we will get an ExpressionChangedAfterItHasBeenCheckedError
    // because the getIndex method will not trigger properly after the currentPage changed to 1.
    this._cdref.detectChanges();

    this.currentPage = 1; // switches current page in pagination back to page 1
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
    var dataURL = canvas.toDataURL("image/jpg");
    return dataURL.replace(/^data:image\/jpg;base64,/, "");
  }

  private addImage(photoURL, doc) {
    let data = `/images/${photoURL}`;
    this.getBase64ImageFromURL(data).subscribe(imageCanvas => {
      let formData = new FormData();
      formData.append('file', imageCanvas);
      let imgconverted = formData.get("file");

      doc.addImage(imgconverted, "JPG", 10, 90, 100, 100);
    });
  }

  private splitText(text) {
    let doc = new jsPDF();
    return doc.splitTextToSize(text, 170);
  }
}
