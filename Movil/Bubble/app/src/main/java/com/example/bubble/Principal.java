package com.example.bubble;

import android.os.Bundle;
import android.view.MenuItem;

import androidx.activity.EdgeToEdge;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.fragment.app.Fragment;

import android.os.Vibrator;
import android.os.VibrationEffect;
import android.os.Build;
import android.content.Context;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.google.android.material.navigation.NavigationBarView;

public class Principal extends AppCompatActivity {


    BottomNavigationView bottomNavigation;
    FloatingActionButton  botonFlotante;

    HomeFragment homeFragment = new HomeFragment();
    FeedFragment feedFragment = new FeedFragment();
    ProfileFragment profileFragment = new ProfileFragment();


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        EdgeToEdge.enable(this);
        setContentView(R.layout.activity_principal);
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main), (v, insets) -> {
            Insets systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars());
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom);
            return insets;
        });

        bottomNavigation = findViewById(R.id.bottomNavigation);
        botonFlotante = findViewById(R.id.fabHome);

        getSupportFragmentManager().beginTransaction().replace(R.id.fragmentContainer, homeFragment).commit();

        bottomNavigation.setOnItemSelectedListener(new com.google.android.material.bottomnavigation.BottomNavigationView.OnItemSelectedListener() {
            @Override
            public boolean onNavigationItemSelected(@NonNull MenuItem item) {
                int id = item.getItemId();

                    if (id == R.id.feed) {
                    getSupportFragmentManager().beginTransaction()
                            .replace(R.id.fragmentContainer, feedFragment)
                            .commit();
                    return true;
                } else if (id == R.id.profile) {
                    getSupportFragmentManager().beginTransaction()
                            .replace(R.id.fragmentContainer, profileFragment)
                            .commit();
                    return true;
                }

                return false;
            }
        });

        botonFlotante.setOnClickListener(v -> {
            Fragment currentFragment = getSupportFragmentManager().findFragmentById(R.id.fragmentContainer);
            if (!(currentFragment instanceof HomeFragment)) {
                getSupportFragmentManager().beginTransaction()
                        .replace(R.id.fragmentContainer, homeFragment)
                        .commit();
            }
        });


        botonFlotante.setOnLongClickListener(v -> {
            Fragment currentFragment = getSupportFragmentManager().findFragmentById(R.id.fragmentContainer);
            if (currentFragment instanceof HomeFragment) {
                vibrarAlCrear();
                ((HomeFragment) currentFragment).crearBurbuja();
                return true;
            }
            return false;
        });





    }
    private void vibrarAlCrear() {
        Vibrator v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);

        if (v != null && v.hasVibrator()) {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {

                v.vibrate(VibrationEffect.createOneShot(50, VibrationEffect.DEFAULT_AMPLITUDE));
            } else {

                v.vibrate(50);
            }
        }
    }
}