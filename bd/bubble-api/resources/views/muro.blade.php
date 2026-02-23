<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <title>Buble - Mapa de Proximidad</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />
    <link href="https://fonts.googleapis.com/css2?family=Roboto+Mono:wght@400;700&display=swap" rel="stylesheet">

    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Roboto Mono', monospace;
            background-color: #131313;
            background-image: url('https://freight.cargo.site/t/original/i/ead9616a26eff00700fd6053e96fd614141f6c9a5ec9ea6080d0fc9a726119d7/swirls-1.gif');
            background-size: cover;
            background-attachment: fixed;
            height: 100vh;
            overflow: hidden;
        }

        .main-layout {
            display: grid;
            grid-template-columns: 300px 1fr 250px;
            height: calc(100vh - 70px);
            gap: 20px;
            padding: 20px;
            box-sizing: border-box;
        }

        .panel {
            background: rgba(255, 255, 255, 0.9);
            backdrop-filter: blur(10px);
            border-radius: 30px;
            padding: 20px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
            overflow-y: auto;
        }


        .panel h3 {
            margin-top: 0;
            color: #050505;
            border-bottom: 2px solid #eee;
            padding-bottom: 10px;
            font-size: 18px;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        .notif-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 12px 10px;
            border-bottom: 1px solid rgba(0, 0, 0, 0.05);
            font-family: 'Roboto Mono', monospace;
            transition: background 0.3s;
        }

        .notif-item:hover {
            background: rgba(59, 76, 202, 0.05);
        }

        .notif-avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            border: 2px solid #3b4cca;
            object-fit: cover;
            flex-shrink: 0;
        }

        .notif-content {
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

        .notif-user {
            font-weight: bold;
            color: #131313;
            font-size: 13px;
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .notif-text {
            font-size: 11px;
            color: #666;
        }

        .notif-distance {
            font-size: 11px;
            color: #2ecc71;
            /* Verde radar */
            font-weight: bold;
            margin-top: 2px;
        }

        #map {
            width: 100%;
            height: 100%;
            border-radius: 30px;
            border: 5px solid white;
            box-sizing: border-box;
        }

        .profile-info {
            text-align: center;
        }

        .profile-info h3 {
            margin: 15px 0 5px 0;
            font-size: 22px;
            color: #131313;
            border: none;
        }

        .profile-info img#avatarPreview {
            width: 100px;
            height: 100px;
            border-radius: 50%;
            border: 3px solid #050505;
        }


        .navbar {
            background: white;
            padding: 10px 5%;
            display: flex;
            justify-content: space-between;
            align-items: center;
            height: 70px;
        }

        .navbar .logo {
            font-weight: bold;
            color: #131313;
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 26px;
        }

        .navbar .logo img {
            height: 45px;
            width: auto;
            object-fit: contain;
        }


        .logo-switch-container {
            margin-top: 30px;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .logo-toggle {
            position: relative;
            display: inline-block;
            cursor: pointer;
        }

        .logo-toggle input {
            position: absolute;
            opacity: 0;
            width: 0;
            height: 0;
            margin: 0;
        }

        .logo-wrapper {
            position: relative;
            display: block;
            width: 80px;
            height: 80px;
            transition: transform 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
        }

        .logo-img {
            width: 100%;
            height: 100%;
            object-fit: contain;
            filter: grayscale(100%) opacity(0.3);
            transition: all 0.4s ease;
        }

        .status-dot {
            position: absolute;
            bottom: 5px;
            right: 5px;
            width: 16px;
            height: 16px;
            background-color: #ff4d4d;
            border: 3px solid white;
            border-radius: 50%;
            z-index: 10;
            transition: all 0.4s ease;
        }

        .logo-toggle input:checked+.logo-wrapper .logo-img {
            filter: grayscale(0%) opacity(1) drop-shadow(0 0 10px rgba(8, 8, 8, 0.5));
            transform: scale(1.1);
        }

        .logo-toggle input:checked+.logo-wrapper .status-dot {
            background-color: #2ecc71;
            box-shadow: 0 0 12px #2ecc71;
        }



        @keyframes spin {
            to {
                transform: rotate(360deg);
            }
        }
    </style>
</head>

<body>

    <div id="loader"
        style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: #131313; display: flex; flex-direction: column; justify-content: center; align-items: center; z-index: 9999; transition: opacity 0.5s ease;">
        <img src="{{ asset('img/logo.gif') }}" width="80" style="margin-bottom: 20px;">
        <div class="spinner"></div>
        <p style="color: white; margin-top: 15px; font-family: Arial;">Localizando burbujas...</p>
    </div>

    <nav class="navbar">
        <div class="logo"><img src="{{ asset('img/logo.gif') }}" width="40"> Bubble</div>

        <form action="{{ route('logout') }}" method="POST" style="margin: 0;">
            @csrf
            <button type="submit"
                style="background: none; border: none; color: red; font-weight: bold; cursor: pointer; font-size: 16px; font-family: 'Roboto Mono', monospace;">
                Cerrar sesión
            </button>
        </form>
    </nav>

    <div class="main-layout">
        <aside class="panel">
            <h3><i class="fas fa-satellite-dish"></i> Radar</h3>
            <div id="contenedor-notificaciones">
                @foreach ($burbujas as $burbuja)
                    <div class="notif-item">
                        <img src="{{ $burbuja->user->avatar ? asset('storage/' . $burbuja->user->avatar) : 'https://ui-avatars.com/api/?name=' . urlencode($burbuja->user->name) . '&background=3b4cca&color=fff' }}"
                            class="notif-avatar">

                        <div class="notif-content">
                            <span class="notif-user">{{ $burbuja->user->name }}</span>
                            <span class="notif-text">Burbuja activa</span>

                            <span class="notif-distance">
                                <i class="fas fa-map-marker-alt"></i>
                                @if ($burbuja->distance < 1)
                                    {{ round($burbuja->distance * 1000) }}m
                                @else
                                    {{ number_format($burbuja->distance, 1) }}km
                                @endif
                            </span>
                        </div>
                    </div>
                @endforeach
            </div>
        </aside>

        <main style="position: relative;">
            <div id="map"></div>
        </main>

        <aside class="panel panel-profile"> <div class="profile-info">
        
        <a href="{{ route('profile.edit') }}" title="Editar perfil">
            <img src="{{ Auth::user()->avatar ? asset('storage/' . Auth::user()->avatar) : 'https://ui-avatars.com/api/?name=' . Auth::user()->name . '&background=3b4cca&color=fff' }}"
                alt="Avatar" id="avatarPreview" style="cursor: pointer; object-fit: cover;">
        </a>

        <h3>{{ Auth::user()->name }}</h3>

        <div class="logo-switch-container">
            <label class="logo-toggle">
                @php
                    $miBurbujaActiva = \App\Models\Bubble::where('user_id', auth()->id())->exists();
                @endphp

                <input type="checkbox" id="btnToggleBurbuja" onchange="gestionarBurbuja(this)"
                    {{ $miBurbujaActiva ? 'checked' : '' }}>

                <div class="logo-wrapper">
                    <img src="{{ asset('img/logo.gif') }}" class="logo-img">
                    <div class="status-dot"></div>
                </div>
            </label>
        </div>

        <form id="formBurbujaToggle" action="" method="POST" style="display: none;">
            @csrf
            <input type="hidden" name="_method" id="metodo_burbuja" value="POST">
            <input type="hidden" name="lat" id="lat_input_switch">
            <input type="hidden" name="lng" id="lng_input_switch">
        </form>
    </div>
</aside>
    </div>

    <script>
        let map;
        let miLatActual = null;
        let miLngActual = null;
        let mapaCentrado = false;
        let ultimaActualizacion = 0;

        function initMap() {
            const centro = {
                lat: 40.4167,
                lng: -3.7037
            };

            map = new google.maps.Map(document.getElementById("map"), {
                zoom: 17,
                center: centro,
                tilt: 45,
                heading: 0,
                mapTypeId: 'roadmap',
                gestureHandling: "greedy",
                disableDefaultUI: true,
            });

            const iconoNavegacion = {
                path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                scale: 6,
                fillColor: "#e74c3c",
                fillOpacity: 1,
                strokeWeight: 2,
                strokeColor: "#ffffff",
                rotation: 0
            };

            @foreach ($burbujas as $burbuja)
                @if ($burbuja->latitude && $burbuja->longitude)
                    (function() {
                        const burbujaPos = {
                            lat: parseFloat("{{ $burbuja->latitude }}"),
                            lng: parseFloat("{{ $burbuja->longitude }}")
                        };
                        new google.maps.Circle({
                            strokeColor: "#3498db",
                            strokeOpacity: 0.8,
                            strokeWeight: 2,
                            fillColor: "#3498db",
                            fillOpacity: 0.35,
                            map: map,
                            center: burbujaPos,
                            radius: 50
                        });
                    })();
                @endif
            @endforeach

            if (navigator.geolocation) {
                navigator.geolocation.watchPosition((position) => {
                    miLatActual = position.coords.latitude;
                    miLngActual = position.coords.longitude;
                    const miPos = {
                        lat: miLatActual,
                        lng: miLngActual
                    };
                    const rumbo = position.coords.heading || 0;

                    const ahora = Date.now();
                    if (ahora - ultimaActualizacion > 2000) {
                        actualizarRadar(miLatActual, miLngActual);
                        ultimaActualizacion = ahora;
                    }

                    if (!mapaCentrado) {
                        map.setCenter(miPos);
                        mapaCentrado = true;
                        const loader = document.getElementById('loader');
                        if (loader) {
                            loader.style.opacity = '0';
                            setTimeout(() => loader.style.display = 'none', 500);
                        }
                    }

                    if (window.miMarcador) {
                        window.miMarcador.setPosition(miPos);
                        iconoNavegacion.rotation = rumbo;
                        window.miMarcador.setIcon(iconoNavegacion);
                    } else {
                        window.miMarcador = new google.maps.Marker({
                            position: miPos,
                            map: map,
                            icon: iconoNavegacion
                        });
                    }

                }, (error) => {
                    console.warn("Error GPS:", error);
                }, {
                    enableHighAccuracy: true,
                    maximumAge: 1000
                });
            }
        }

        function actualizarRadar(lat, lng) {
            fetch(`/api/notificaciones?lat=${lat}&lng=${lng}`)
                .then(response => {
                    if (!response.ok) throw new Error("Error de red");
                    return response.text();
                })
                .then(data => {
                    data = data.trim();

                    if (data.startsWith('HJ') || data.length < 5) {
                        console.warn("Datos corruptos ignorados:", data);
                        return;
                    }

                    console.log("Datos recibidos correctamente:", data.substring(0, 50) + "...");

                    const contenedor = document.getElementById('contenedor-notificaciones');
                    if (contenedor) {
                        contenedor.innerHTML = data;
                    }
                })
                .catch(err => console.error("Error en radar:", err));
        }

        function gestionarBurbuja(checkbox) {
            const form = document.getElementById('formBurbujaToggle');
            const metodo = document.getElementById('metodo_burbuja');

            if (checkbox.checked) {
                if (miLatActual !== null) {
                    form.action = "{{ route('burbuja.crearBurbuja') }}";
                    metodo.value = "POST";
                    document.getElementById('lat_input_switch').value = miLatActual;
                    document.getElementById('lng_input_switch').value = miLngActual;
                    form.submit();
                } else {
                    alert("Espera a que te localicemos...");
                    checkbox.checked = false;
                }
            } else {
                form.action = "{{ route('burbuja.eliminarBurbuja') }}";
                metodo.value = "DELETE";
                form.submit();
            }
        }
    </script>
    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDrNWfOEQS3UZ6-7wEQHLv40__DLg5BF6E&callback=initMap" async
        defer></script>
</body>

</html>
