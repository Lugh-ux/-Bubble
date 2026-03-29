<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Storage;

class ControladorPerfilEscritorio extends Controller
{
    public function show(Request $request)
    {
        $user = $request->user();
        $bubbleCount = $user->bubbles()->count();

        return response()->json([
            'user' => [
                'id' => $user->id,
                'name' => $user->name,
                'email' => $user->email,
                'avatar_url' => $user->avatar ? asset('storage/' . $user->avatar) : null,
            ],
            'stats' => [
                'bubble_count' => $bubbleCount,
            ],
        ]);
    }

    public function update(Request $request)
    {
        $data = $request->validate([
            'name' => 'required|string|max:255',
            'avatar' => 'nullable|image|mimes:jpeg,png,jpg,gif|max:4096',
        ]);

        $user = $request->user();
        $user->name = $data['name'];

        if ($request->hasFile('avatar')) {
            if ($user->avatar) {
                Storage::disk('public')->delete($user->avatar);
            }

            $user->avatar = $request->file('avatar')->store('avatars', 'public');
        }

        $user->save();

        return response()->json([
            'message' => 'Perfil actualizado.',
            'user' => [
                'id' => $user->id,
                'name' => $user->name,
                'email' => $user->email,
                'avatar_url' => $user->avatar ? asset('storage/' . $user->avatar) : null,
            ],
        ]);
    }
}
