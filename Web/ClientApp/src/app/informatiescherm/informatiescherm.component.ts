import {Component, OnInit, ElementRef, ViewChild} from '@angular/core';
import * as jsPDF from 'jspdf';
import * as html2canvas from 'html2canvas';

// @ts-ignore

@Component({
  selector: 'app-informatiescherm',
  templateUrl: './informatiescherm.component.html',
  styleUrls: ['./informatiescherm.component.css']
})
export class InformatieschermComponent implements OnInit {
  /**
   * @ignore
   */
  private _buttonDisabled: boolean;

  /**
   * Sets button on disabled on init
   */
  ngOnInit() {
    this._buttonDisabled = false;
  }

  /**
   * Click event toggles button
   */
  toggleButtonDisabled() {
    this._buttonDisabled = !this._buttonDisabled;
  }

  /**
   * Getter for ButtonDisabled
   */
  get buttonDisabled() {
    return this._buttonDisabled;
  }

  /**
   * @ignore
   */
  @ViewChild('content') content: ElementRef;

  /**
   * This is a method that generates a pdf of the informationscreen
   */
  public downloadPDF() {
    var data = document.getElementById('contentToConvert');
    html2canvas(data).then(canvas => {
      // Few necessary setting options
      var imgWidth = 208;
      var pageHeight = 295;
      var imgHeight = canvas.height * imgWidth / canvas.width;
      var heightLeft = imgHeight;

      const contentDataURL = canvas.toDataURL('image/png');
      console.log(contentDataURL);
      let pdf = new jsPDF('p', 'mm', 'a4'); // A4 size page of PDF
      var position = 0;
      pdf.addImage(contentDataURL, 'PNG', 0, position, imgWidth, imgHeight);
      pdf.save('informatie_applicatie.pdf'); // Generated PDF
    });
  }
}
