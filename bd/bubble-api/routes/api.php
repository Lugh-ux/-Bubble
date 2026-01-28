<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

Route::get('/user', function (Request $request) {
    return $request->user();
});

Route::post('/burbujas', [App\Http\Controllers\Api\BubbleController::class, 'store']);
Route::get('/burbujas', [App\Http\Controllers\Api\BubbleController::class, 'index']);