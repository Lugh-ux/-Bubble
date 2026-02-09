<!DOCTYPE html>
<html lang="es">

<head>
    <meta charset="UTF-8">
    <title>Buble - Mapa de Proximidad</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />

    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: 'Arial', sans-serif;
            background-color: #131313;
            background-image: url('https://freight.cargo.site/t/original/i/ead9616a26eff00700fd6053e96fd614141f6c9a5ec9ea6080d0fc9a726119d7/swirls-1.gif');
            background-size: cover;
            background-attachment: fixed;
            height: 100vh;
            overflow: hidden;
        }

        .main-layout {
            display: grid;
            grid-template-columns: 300px 1fr 300px;
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

        #map {
            width: 100%;
            height: 100%;
            border-radius: 30px;
            border: 5px solid white;
            box-sizing: border-box;
        }

        .panel h3 {
            margin-top: 0;
            color: #000000;
            border-bottom: 1px solid #eee;
            padding-bottom: 10px;
        }

        .notif-item {
            padding: 10px;
            border-bottom: 1px solid #f0f0f0;
            font-size: 14px;
        }

        .profile-info {
            text-align: center;
        }

        .profile-info img {
            width: 100px;
            height: 100px;
            border-radius: 50%;
            border: 3px solid #3b4cca;
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
            color: #000000;
            display: flex;
            align-items: center;
            gap: 10px;
        }
    </style>
</head>

<body>

    <nav class="navbar">
        <div class="logo"><img src="{{ asset('img/logo.gif') }}" width="40"> Bubble</div>
        <a href="#" style="text-decoration: none; color: red; font-weight: bold;">Cerrar sesión</a>
    </nav>

    <div class="main-layout">
        <aside class="panel">
            <h3><i class="fas fa-bell"></i> Entorno</h3>
            <div class="notif-item"> <b>Iker</b> Burbuja nueva</div>
            <div class="notif-item"> <b>Test</b>Esta cerca</div>
            <div class="notif-item"> Notificacion de prueba</div>
        </aside>

        <main style="position: relative;">
            <div id="map"></div>
        </main>

        <aside class="panel">
    <div class="profile-info">
        <div style="position: relative; display: inline-block;">
            <img src="{{ Auth::user()->avatar ? asset('storage/' . Auth::user()->avatar) : 'https://ui-avatars.com/api/?name=' . Auth::user()->name . '&background=3b4cca&color=fff' }}" 
                 alt="Avatar" id="avatarPreview" style="cursor: pointer; object-fit: cover;">
            
            <form action="{{ route('perfil.update') }}" method="POST" enctype="multipart/form-data" id="formAvatar">
                @csrf
                @method('PATCH')
                <input type="file" name="caratula" id="inputAvatar" style="display:none;" accept="image/*" onchange="document.getElementById('formAvatar').submit();">
                <label for="inputAvatar" style="position: absolute; bottom: 0; right: 0; background: white; border-radius: 50%; width: 30px; height: 30px; display: flex; align-items: center; justify-content: center; cursor: pointer; border: 1px solid #ccc;">
                    <i class="fas fa-camera" style="color: #3b4cca; font-size: 14px;"></i>
                </label>
            </form>
        </div>

        <h3>{{ Auth::user()->name }}</h3>
        <p style="color: #666; font-size: 14px;">{{ Auth::user()->email }}</p>
        
        <hr>
        <div style="text-align: left;">
            <p><i class="fas fa-comment-dots" style="color: #3b4cca;"></i> Burbujas: {{ count($burbujas->where('user_id', Auth::id())) }}</p>
            <p><i class="fas fa-users" style="color: #3b4cca;"></i> Amigos: 45</p>
        </div>
    </div>
</aside>
    </div>

    <script>
        let map;

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
                tiltControl: true,
                rotateControl: true
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

            console.log("Mapa cargado. Empezando a pintar burbujas...");
            console.log("Pintando burbujas con nombres corregidos...");

            @foreach ($burbujas as $burbuja)
                @if ($burbuja->latitude && $burbuja->longitude)
                    (function() {
                        const burbujaPos = {
                            lat: parseFloat("{{ $burbuja->latitude }}"),
                            lng: parseFloat("{{ $burbuja->longitude }}")
                        };

                        const circuloBurbuja = new google.maps.Circle({
                            strokeColor: "#3498db",
                            strokeOpacity: 0.8,
                            strokeWeight: 2,
                            fillColor: "#3498db",
                            fillOpacity: 0.35,
                            map: map,
                            center: burbujaPos,
                            radius: 50
                        });

                        const iw = new google.maps.InfoWindow({
                            content: `<div style="color:black; padding:5px;">
                            <strong>{{ $burbuja->user->name ?? 'Anónimo' }}</strong><br>
                            {{ $burbuja->mensaje }}
                          </div>`,
                            position: burbujaPos
                        });

                        circuloBurbuja.addListener("click", () => {
                            iw.open(map);
                        });
                    })();
                @endif
            @endforeach

            if (navigator.geolocation) {
                navigator.geolocation.watchPosition((position) => {
                    const miPos = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };

                    const rumbo = position.coords.heading || 0;

                    map.setCenter(miPos);
                    map.setTilt(45); 
                    map.setHeading(rumbo); 

                    if (window.miMarcador) {
                        window.miMarcador.setPosition(miPos);
                        iconoNavegacion.rotation = rumbo;
                        window.miMarcador.setIcon(iconoNavegacion);
                    } else {
                        iconoNavegacion.rotation = rumbo;
                        window.miMarcador = new google.maps.Marker({
                            position: miPos,
                            map: map,
                            icon: iconoNavegacion,
                            title: "Tu dirección"
                        });
                    }
                }, (error) => {
                    console.warn("Error de GPS: ", error);
                }, {
                    enableHighAccuracy: true, 
                    maximumAge: 1000
                });
            }
        }
    </script>

    <script src="https://maps.googleapis.com/maps/api/js?key=AIzaSyDrNWfOEQS3UZ6-7wEQHLv40__DLg5BF6E&callback=initMap" async
        defer></script>

</body>

</html>
