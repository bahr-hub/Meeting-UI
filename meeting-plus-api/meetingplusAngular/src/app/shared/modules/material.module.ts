import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatMenuModule } from '@angular/material/menu';

@NgModule({
    imports: [
        // MatSidenavModule,
        MatMenuModule,
        MatIconModule,
        BrowserAnimationsModule,
        MatProgressSpinnerModule,
        MatDialogModule,
        MatButtonModule,
        MatDividerModule,
    ],
    exports: [
        // MatSidenavModule,
        MatMenuModule,
        MatIconModule,
        BrowserAnimationsModule,
        MatProgressSpinnerModule,
        MatDialogModule,
        MatButtonModule,
        MatDividerModule,
        MatButtonModule,

    ]
})
export class MaterialModule { }