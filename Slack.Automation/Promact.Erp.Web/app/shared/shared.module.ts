import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Md2Module } from 'md2';
import { MaterialModule } from '@angular/material';

@NgModule({
    imports: [
        CommonModule,
        Md2Module.forRoot(),
        MaterialModule.forRoot()
    ],
    exports: [CommonModule, FormsModule, Md2Module, MaterialModule]
})
export class SharedModule { }