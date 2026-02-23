<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\BubbleController;
use App\Http\Controllers\Api\ControladorRegistro;
use App\Http\Controllers\ProfileController;


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

Route::patch('/perfil/update', [BubbleController::class, 'updatePerfil'])->name('perfil.update');

Route::post('/burbuja', [BubbleController::class, 'crearBurbuja'])->name('burbuja.crearBurbuja');
Route::delete('/burbuja', [BubbleController::class, 'eliminarBurbuja'])->name('burbuja.eliminarBurbuja');

Route::get('/api/notificaciones', [BubbleController::class, 'getNotificacionesAjax'])->name('notificaciones.ajax');

Route::middleware('auth')->group(function () {
    Route::get('/perfil', [ProfileController::class, 'edit'])->name('profile.edit');
    
    Route::patch('/perfil', function(\Illuminate\Http\Request $request) {
    dd('¡BINGO! HE LLEGADO A LA RUTA. Datos enviados:', $request->all());
})->name('perfil.update');
});
Route::post('/guardar-mi-perfil-nuevo', [App\Http\Controllers\ProfileController::class, 'update']);