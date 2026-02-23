<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Storage;
use Illuminate\Support\Facades\Auth;
use App\Models\User;

class ProfileController extends Controller
{
    
    public function edit()
    {
        return view('perfil'); 
    }

    
    public function update(Request $request)
    {
        $request->validate([
            'name' => 'required|string|max:255',
            'avatar' => 'nullable|image|mimes:jpeg,png,jpg,gif|max:2048', 
        ]);

        /** @var \App\Models\User $user */
        $user = Auth::user();
        $user->name = $request->name;

        if ($request->hasFile('avatar')) {
            if ($user->avatar) {
                Storage::disk('public')->delete($user->avatar);
            }
            $rutaImagen = $request->file('avatar')->store('avatars', 'public');
            $user->avatar = $rutaImagen;
        }

        $user->save();

        return redirect('/muro')->with('status', '¡Perfil actualizado con éxito!');
    }
}