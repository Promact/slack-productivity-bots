import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Md2Module } from 'md2';

@NgModule({
    imports: [
        CommonModule,
        Md2Module.forRoot()
    ],
    exports: [CommonModule, FormsModule, Md2Module]
})
export class SharedModule { }