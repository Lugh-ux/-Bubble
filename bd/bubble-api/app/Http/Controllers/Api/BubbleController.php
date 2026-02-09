<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;
use App\Models\Bubble;
use App\Models\User;
use Illuminate\Support\Facades\Storage;
use Illuminate\Support\Facades\Auth;

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

    public function mostrarMuro()
    {
        $burbujas = Bubble::with('user')->latest()->get();

        return view('muro', compact('burbujas'));
    }

    public function updatePerfil(Request $request)
    {
        $request->validate([
            'foto' => 'required|image|mimes:png,jpg|max:5000'
        ]);

        $usuario = User::find(Auth::id());

        if (!$usuario) {
            
            return redirect()->back()->with('error', 'Usuario no autenticado.');
        }

        if ($request->hasFile('foto')) {
            $file = $request->file('foto');

            $rutaImagen = $file->store('avatars', ['disk' => 'public']);

            if ($usuario->avatar) {
                Storage::disk('public')->delete($usuario->avatar);
            }

            $usuario->avatar = $rutaImagen;

            $usuario->save();
        }

        return redirect()->back()->with('success', 'Foto de perfil actualizada.');
    }
}
