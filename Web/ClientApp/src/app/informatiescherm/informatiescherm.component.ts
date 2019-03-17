import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
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
      var imgData = canvas.toDataURL('image/png');
      var imgWidth = 210;
      var pageHeight = 295;
      var imgHeight = canvas.height * imgWidth / canvas.width;
      var heightLeft = imgHeight;
      var doc = new jsPDF('p', 'mm');
      var position = 0;

      doc.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
      heightLeft -= pageHeight;

      while (heightLeft >= 0) {
        position = heightLeft - imgHeight;
        doc.addPage();
        doc.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
        heightLeft -= pageHeight;
      }


      /*let pdf = new jsPDF('p', 'pt', 'a4'); // A4 size page of PDF
      let wid: number;
      let hgt: number;
      let img = canvas.toDataURL("image/png", wid = canvas.width, hgt = canvas.height);
      let ratio = hgt / wid;
      let width = pdf.internal.pageSize.width - 20;
      let height = width * ratio - 20;
      pdf.addImage(img, 'JPEG', 20, 20, width, height);
      pdf.addPage();
      pdf.addImage(img, 'JPEG', 20, 20, width, height);*/
      doc.save('informatie_applicatie.pdf'); // Generated PDF
    });
  }
}
