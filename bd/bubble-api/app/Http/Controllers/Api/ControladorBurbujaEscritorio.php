<?php

namespace App\Http\Controllers\Api;

use App\Http\Controllers\Controller;
use App\Models\Bubble;
use Illuminate\Http\Request;

class ControladorBurbujaEscritorio extends Controller
{
    public function dashboard(Request $request)
    {
        $request->validate([
            'lat' => 'nullable|numeric',
            'lng' => 'nullable|numeric',
        ]);

        $lat = (float) $request->query('lat', 40.4167);
        $lng = (float) $request->query('lng', -3.7037);
        $user = $request->user();
        $myBubble = Bubble::with('user')->where('user_id', $user->id)->first();

        $bubbles = $this->nearbyBubbles($lat, $lng, $user->id)
            ->map(fn (Bubble $bubble) => $this->formatBubble($bubble));

        // Evita 500 si hay textos con bytes invalidos en BD (Malformed UTF-8 characters).
        return response()->json([
            'current_user' => [
                'id' => $user->id,
                'name' => $user->name,
                'email' => $user->email,
                'avatar_url' => $user->avatar ? asset('storage/' . $user->avatar) : null,
            ],
            'my_bubble_active' => $myBubble !== null,
            'my_bubble' => $myBubble ? $this->formatBubble($myBubble, true) : null,
            'bubbles' => $bubbles->values(),
        ], 200, [], JSON_INVALID_UTF8_SUBSTITUTE);
    }

    public function store(Request $request)
    {
        $data = $request->validate([
            'lat' => 'required|numeric',
            'lng' => 'required|numeric',
        ]);

        Bubble::where('user_id', $request->user()->id)->delete();

        $bubble = Bubble::create([
            'user_id' => $request->user()->id,
            'mensaje' => 'Burbuja activa!',
            'latitude' => $data['lat'],
            'longitude' => $data['lng'],
        ]);

        return response()->json([
            'message' => 'Burbuja activada.',
            'bubble' => $this->formatBubble($bubble->load('user'), true),
        ], 201);
    }

    public function destroy(Request $request)
    {
        Bubble::where('user_id', $request->user()->id)->delete();

        return response()->json([
            'message' => 'Burbuja eliminada.',
        ]);
    }

    private function nearbyBubbles(float $lat, float $lng, int $currentUserId)
    {
        return Bubble::with('user')
            ->whereHas('user')
            ->where('user_id', '!=', $currentUserId)
            ->whereNotNull('latitude')
            ->whereNotNull('longitude')
            ->selectRaw(
                "*, (
                    6371 * acos(
                        cos(radians(?)) * cos(radians(latitude)) * cos(radians(longitude) - radians(?)) +
                        sin(radians(?)) * sin(radians(latitude))
                    )
                ) AS distance",
                [$lat, $lng, $lat]
            )
            ->orderBy('distance')
            ->take(30)
            ->get();
    }

    private function formatBubble(Bubble $bubble, bool $isCurrentUser = false): array
    {
        $distance = (float) ($bubble->distance ?? 0);
        $user = $bubble->user;

        return [
            'id' => $bubble->id,
            'message' => $bubble->mensaje ?: 'Burbuja activa',
            'latitude' => (float) $bubble->latitude,
            'longitude' => (float) $bubble->longitude,
            'distance_km' => round($distance, 3),
            'distance_label' => $distance < 1
                ? round($distance * 1000) . ' m'
                : number_format($distance, 1) . ' km',
            'is_current_user' => $isCurrentUser,
            'user' => [
                // Si hay datos inconsistentes (burbuja huérfana), evitamos 500.
                'id' => $user?->id ?? 0,
                'name' => $user?->name ?? 'Usuario',
                'avatar_url' => $user && $user->avatar ? asset('storage/' . $user->avatar) : null,
            ],
        ];
    }
}
