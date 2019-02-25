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
      let pdf = new jsPDF('p', 'pt', 'a4'); // A4 size page of PDF
      let wid: number;
      let hgt: number;
      let img = canvas.toDataURL("image/png", wid = canvas.width, hgt = canvas.height);
      let ratio = hgt / wid;
      let width = pdf.internal.pageSize.width - 20;
      let height = width * ratio - 20;
      pdf.addImage(img, 'JPEG', 20, 20, width, height);
      pdf.addImage(img, 'JPEG', 20, 20, width, height);
      pdf.save('informatie_applicatie.pdf'); // Generated PDF
    });
  }
}
