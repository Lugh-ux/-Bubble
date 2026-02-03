<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\BubbleController;
use App\Http\Controllers\Api\ControladorRegistro;


Route::get('/', function () {
    return view('welcome');
});

Route::get('/', [BubbleController::class, 'index']); 

Route::get('/login', [ControladorRegistro::class, 'showLogin'])->name('login');
Route::post('/login', [ControladorRegistro::class, 'login'])->name('login.post');
Route::get('/registro', [ControladorRegistro::class, 'showRegistro'])->name('registro');
Route::post('/registro', [ControladorRegistro::class, 'registro'])->name('registro.post');

Route::middleware(['auth'])->group(function () {
    Route::get('/muro', [BubbleController::class, 'mostrarMuro'])->name('muro');
    Route::post('/logout', [ControladorRegistro::class, 'logout'])->name('logout');
    Route::post('/burbujas/guardar', [BubbleController::class, 'guardar']);
});

Route::get('/', function () { return redirect()->route('login'); });


