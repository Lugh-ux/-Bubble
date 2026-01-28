<?php

namespace Database\Seeders;

use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;
use App\Models\User;
use App\Models\Bubble;
use Illuminate\Support\Facades\Hash;

class BubbleSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {

        $user = User::create([
            'name' => 'Iker',
            'email' => 'iker@bubble.com',
            'password' => Hash::make('123456'),
        ]);

        
        Bubble::create([
            'user_id' => $user->id,
            'latitude' => 42.2365,
            'longitude' => -8.7142,
            'mensaje' => '¡Primera burbuja en el Castro!'
        ]);

        Bubble::create([
            'user_id' => $user->id,
            'latitude' => 42.2394,
            'longitude' => -8.7201,
            'mensaje' => 'Cerca de la Puerta del Sol'
        ]);
    }
}
