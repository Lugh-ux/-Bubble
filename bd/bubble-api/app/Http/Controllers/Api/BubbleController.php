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
    public function index(Request $request)
    {

        $latUsuario = $request->get('lat', 0);
        $lngUsuario = $request->get('lng', 0);

        $burbujas = \App\Models\Bubble::with('user')
            ->selectRaw(
                "*, 
            (6371 * acos(cos(radians(?)) * cos(radians(latitude)) * cos(radians(longitude) - radians(?)) + sin(radians(?)) * sin(radians(latitude)))) AS distance",
                [$latUsuario, $lngUsuario, $latUsuario]
            )
            ->orderBy('distance', 'asc')
            ->get();

        return view('tu_vista', compact('burbujas'));
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
            'caratula' => 'required|image|mimes:png,jpg|max:5000'
        ]);

        $usuario = Auth::user();

        if ($request->hasFile('caratula')) {
            $file = $request->file('caratula');
            $rutaImagen = $file->store('caratulas', ['disk' => 'public']);

            if ($usuario->avatar) {
                Storage::disk('public')->delete($usuario->avatar);
            }

            $usuario->avatar = $rutaImagen;
            $usuario->save();
        }

        return redirect()->back()->with('success', 'Foto actualizada');
    }

    public function crearBurbuja(Request $request)
    {
        try {
            $request->validate([
                'lat' => 'required',
                'lng' => 'required'
            ]);

            \App\Models\Bubble::where('user_id', auth()->id())->delete();

            \App\Models\Bubble::create([
                'user_id'   => auth()->id(),
                'mensaje'   => "¡Burbuja activa!",
                'latitude'  => $request->lat,
                'longitude' => $request->lng,
            ]);

            return redirect()->back()->with('success', 'Burbuja actualizada.');
        } catch (\Exception $e) {
            dd("Error al crear burbuja: " . $e->getMessage());
        }
    }

    public function eliminarBurbuja()
    {
        Bubble::where('user_id', Auth::id())->delete();
        return redirect()->back()->with('success', 'Burbuja eliminada.');
    }

    public function getNotificacionesAjax(Request $request)
{
    if (ob_get_length()) ob_clean();

    $latRaw = $request->input('lat', '0');
    $lngRaw = $request->input('lng', '0');
    
    $lat = (float) str_replace(',', '.', $latRaw);
    $lng = (float) str_replace(',', '.', $lngRaw);

    $burbujas = \App\Models\Bubble::with('user')
        ->where('user_id', '!=', auth()->id()) 
        ->whereNotNull('latitude')
        ->whereNotNull('longitude')
        ->selectRaw("*, (
            6371 * acos(
                cos(radians(?)) * cos(radians(latitude)) * cos(radians(longitude) - radians(?)) + 
                sin(radians(?)) * sin(radians(latitude))
            )
        ) AS distance", [$lat, $lng, $lat])
        ->orderBy('distance', 'asc')
        ->take(20)
        ->get();

    $html = "";
    

    if ($burbujas->isEmpty()) {
        $html = "<div class='notif-item' style='justify-content:center; color:#999; padding:20px;'>
                    <i class='fas fa-wind'></i> No hay burbujas cerca
                 </div>";
    } else {
        foreach ($burbujas as $b) {
            
            $nombreAvatar = $b->user->avatar;
            
            
            if (strlen($nombreAvatar) > 250) {
                $nombreAvatar = null; 
            }
            
            $avatar = $nombreAvatar 
                ? asset('storage/' . $nombreAvatar) 
                : 'https://ui-avatars.com/api/?name=' . urlencode($b->user->name) . '&background=3b4cca&color=fff';
            

            if ($b->distance < 1) {
                $distancia = round($b->distance * 1000) . " m";
            } else {
                $distancia = number_format($b->distance, 1) . " km";
            }

            $html .= "
            <div class='notif-item'>
                <img src='{$avatar}' class='notif-avatar' alt='user'>
                <div class='notif-content'>
                    <span class='notif-user'>{$b->user->name}</span>
                    <span class='notif-text'>Burbuja activa</span>
                    <span class='notif-distance'><i class='fas fa-location-arrow'></i> {$distancia}</span>
                </div>
            </div>";
        }
    }
    
    

    return response($html)->header('Content-Type', 'text/html');
}
}
