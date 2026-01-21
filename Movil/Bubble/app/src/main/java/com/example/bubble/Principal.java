package com.example.bubble;

import android.os.Bundle;
import android.view.MenuItem;

import androidx.activity.EdgeToEdge;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.graphics.Insets;
import androidx.core.view.ViewCompat;
import androidx.core.view.WindowInsetsCompat;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.navigation.NavigationBarView;

public class Principal extends AppCompatActivity {


    BottomNavigationView bottomNavigation;

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

        getSupportFragmentManager().beginTransaction().replace(R.id.fragmentContainer, homeFragment).commit();

        bottomNavigation.setOnItemSelectedListener(new com.google.android.material.bottomnavigation.BottomNavigationView.OnItemSelectedListener() {
            @Override
            public boolean onNavigationItemSelected(@NonNull MenuItem item) {
                int id = item.getItemId();

                if (id == R.id.casa) {
                    getSupportFragmentManager().beginTransaction()
                            .replace(R.id.fragmentContainer, homeFragment)
                            .commit();
                    return true;
                } else if (id == R.id.feed) {
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
    }
}