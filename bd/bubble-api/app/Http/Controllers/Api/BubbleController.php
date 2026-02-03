<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;
use App\Models\Bubble;

class BubbleController extends Controller
{
    public function index()
    {
        $burbujas = \App\Models\Bubble::with('user')->get();
        return view('muro', compact('burbujas'));
    }

    public function guardar(Request $request)
    {
        $bubble = new Bubble();
        $bubble->user_id = 1;
        $bubble->latitude = $request->latitude;
        $bubble->longitude = $request->longitude;
        $bubble->save();

        return response()->json([
            'message' => 'Burbuja guardada con éxito',
            'data' => $bubble
        ], 201);
    }
}
