<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;
use App\Http\Controllers\Api\ControladorAutenticacionEscritorio;
use App\Http\Controllers\Api\ControladorBurbujaEscritorio;
use App\Http\Controllers\Api\ControladorPerfilEscritorio;

Route::get('/user', function (Request $request) {
    return $request->user();
});

Route::post('/burbujas', [App\Http\Controllers\Api\BubbleController::class, 'store']);
Route::get('/burbujas', [App\Http\Controllers\Api\BubbleController::class, 'index']);

Route::prefix('desktop')->group(function () {
    Route::post('/login', [ControladorAutenticacionEscritorio::class, 'login']);
    Route::post('/register', [ControladorAutenticacionEscritorio::class, 'register']);

    Route::middleware('auth:sanctum')->group(function () {
        Route::post('/logout', [ControladorAutenticacionEscritorio::class, 'logout']);
        Route::get('/profile', [ControladorPerfilEscritorio::class, 'show']);
        Route::post('/profile', [ControladorPerfilEscritorio::class, 'update']);
        Route::get('/dashboard', [ControladorBurbujaEscritorio::class, 'dashboard']);
        Route::post('/bubble', [ControladorBurbujaEscritorio::class, 'store']);
        Route::delete('/bubble', [ControladorBurbujaEscritorio::class, 'destroy']);
    });
});
